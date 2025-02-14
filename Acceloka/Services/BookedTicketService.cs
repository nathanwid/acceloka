using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Models.Responses;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace Acceloka.Services
{
    public class BookedTicketService
    {
        private readonly AccelokaContext _db;

        public BookedTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<List<GetBookedTicketsResponse>> GetBookedTickets(int bookedTicketId)
        {
            var bookedTicket = await _db.BookedTickets
                .Include(Q => Q.Details)
                .ThenInclude(R => R.Ticket)
                .ThenInclude(S => S.Category)
                .Where(Q => Q.Id == bookedTicketId)
                .FirstOrDefaultAsync();

            if (bookedTicket == null)
            {
                throw new KeyNotFoundException($"Booking {bookedTicketId} tidak ditemukan");
            }

            var response = bookedTicket.Details
                .GroupBy(Q => Q.Ticket.Category.Name)
                .Select(R => new GetBookedTicketsResponse
                {
                    QtyPerCategory = R.Sum(Q => Q.Quantity),
                    CategoryName = R.Key,
                    Tickets = R.Select(Q => new TicketDetailResponse
                    {
                        TicketCode = Q.TicketCode,
                        TicketName = Q.Ticket.Name,
                        EventDate = Q.Ticket.Date.ToString("dd-MM-yyyy HH:mm")
                    }).ToList()
                }).ToList();

            return response;
        }

        public async Task<byte[]> GenerateExcelReport()
        {
            var bookedTickets = await _db.BookedTickets
                .Include(Q => Q.Details)
                .ThenInclude(R => R.Ticket)
                .ThenInclude(S => S.Category)
                .ToListAsync();

            if (!bookedTickets.Any()) 
            {
                throw new KeyNotFoundException("Tidak ada tiket yang dipesan");
            }

            var dataTable = new DataTable("BookedTickets");

            dataTable.Columns.Add("Booked Ticket ID", typeof(int));
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Category", typeof(string));
            dataTable.Columns.Add("Ticket Code", typeof(string));
            dataTable.Columns.Add("Ticket Name", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("Price", typeof(int));
            dataTable.Columns.Add("Total Price", typeof(int));

            foreach (var bookedTicket in bookedTickets)
            {
                var totalBookedPrice = 0; 

                foreach (var detail in bookedTicket.Details)
                {
                    var detailPrice = detail.Quantity * detail.Ticket.Price;
                    totalBookedPrice += detailPrice;

                    dataTable.Rows.Add(
                        bookedTicket.Id,
                        bookedTicket.Date.ToString("dd-MM-yyyy HH:mm"),
                        detail.Ticket.Category.Name,
                        detail.TicketCode,
                        detail.Ticket.Name,
                        detail.Quantity,
                        detail.Ticket.Price,
                        detailPrice
                    );
                }

                dataTable.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, totalBookedPrice);
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(dataTable);
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<BookTicketsResponse> BookTickets(BookedTicketModel request)
        {
            var ticketCodes = request.Tickets.Select(Q => Q.TicketCode).ToList();

            var tickets = await _db.Tickets
                .Where(Q => ticketCodes.Contains(Q.Code))
                .Include(Q => Q.Category)
                .ToListAsync();

            var details = new List<Detail>();

            var totalPrice = 0;

            if (tickets.Count != ticketCodes.Count)
            {
                throw new ArgumentException("Beberapa kode tiket tidak ditemukan");
            }

            foreach (var detail in request.Tickets)
            {
                var ticket = tickets.FirstOrDefault(Q => Q.Code == detail.TicketCode.ToUpper());

                if (ticket == null)
                {
                    throw new ArgumentException($"Kode tiket {detail.TicketCode.ToUpper()} tidak ditemukan");
                }

                if (ticket.Quota <= 0)
                {
                    throw new ArgumentException($"Tiket {ticket.Name} sudah habis");
                }

                if (detail.Quantity <= 0)
                {
                    throw new ArgumentException($"Jumlah tiket tidak valid");
                }

                if (detail.Quantity > ticket.Quota)
                {
                    throw new ArgumentException($"Jumlah tiket melebihi sisa kuota ({ticket.Quota})");
                }

                if (DateTime.Now > ticket.Date)
                {
                    throw new ArgumentException($"Event {ticket.Name} sudah lewat");
                }

                ticket.Quota -= detail.Quantity;

                details.Add(new Detail
                {
                    TicketCode = ticket.Code,
                    Quantity = detail.Quantity
                });
            }

            totalPrice = details.Sum(Q => tickets.First(R => R.Code == Q.TicketCode).Price * Q.Quantity);

            var bookedTicket = new BookedTicket
            {
                Date = DateTime.Now
            };

            _db.BookedTickets.Add(bookedTicket);
            await _db.SaveChangesAsync();

            foreach (var detail in details)
            {
                detail.BookedTicketId = bookedTicket.Id;
            }

            _db.Details.AddRange(details);
            await _db.SaveChangesAsync();

            var response = new BookTicketsResponse
            {
                PriceSummary = totalPrice,
                TicketsPerCategories = tickets
                    .GroupBy(Q => Q.Category.Name)
                    .Select(R => new CategorySummaryResponse
                    {
                        CategoryName = R.Key,
                        SummaryPrice = R.Sum(Q => Q.Price * details
                            .Where(S => S.TicketCode == Q.Code)
                            .Sum(S => S.Quantity)),
                        Tickets = R.Select(Q => new TicketSummaryResponse
                        {
                            TicketCode = Q.Code,
                            TicketName = Q.Name,
                            Price = Q.Price
                        }).ToList()
                    }).ToList()
            };

            return response;
        }

        public async Task<List<ModifyTicketResponse>> EditBookedTicket(int bookedTicketId, BookedTicketModel request)
        {
            var bookedTicket = await _db.BookedTickets
                .Include(Q => Q.Details)
                .ThenInclude(R => R.Ticket)
                .ThenInclude(S => S.Category)
                .Where(Q => Q.Id == bookedTicketId)
                .FirstOrDefaultAsync();

            if (bookedTicket == null)
            {
                throw new KeyNotFoundException($"Booking {bookedTicketId} tidak ditemukan");
            }

            foreach (var detailUpdate in request.Tickets)
            {
                var detail = bookedTicket.Details
                    .Where(Q => Q.TicketCode == detailUpdate.TicketCode.ToUpper())
                    .FirstOrDefault();

                if (detail == null)
                {
                    throw new KeyNotFoundException($"Kode tiket {detailUpdate.TicketCode.ToUpper()} tidak ditemukan");
                }

                if (detailUpdate.Quantity == detail.Quantity)
                {
                    continue;
                }

                if (detailUpdate.Quantity > detail.Ticket.Quota)
                {
                    throw new ArgumentException($"Jumlah tiket yang dimasukkan untuk tiket {detail.TicketCode} melebihi kuota ({detail.Ticket.Quota})");
                }

                if (detailUpdate.Quantity < 1)
                {
                    throw new ArgumentException($"Jumlah tiket yang dimasukkan untuk tiket {detail.TicketCode} kurang dari 1");
                }

                int quantityChange = detailUpdate.Quantity - detail.Quantity;
                detail.Ticket.Quota -= quantityChange;

                detail.Quantity = detailUpdate.Quantity;

                _db.Update(detail);
            }

            await _db.SaveChangesAsync();

            var response = bookedTicket.Details
                .Select(Q => new ModifyTicketResponse
                {
                    TicketCode = Q.TicketCode,
                    TicketName = Q.Ticket.Name,
                    Quantity = Q.Quantity,
                    CategoryName = Q.Ticket.Category.Name
                }).ToList();

            return response;
        }

        public async Task<List<ModifyTicketResponse>> RevokeTicket(int bookedTicketId, string ticketCode, int quantity)
        {
            var bookedTicket = await _db.BookedTickets
                .Include(Q => Q.Details)
                .ThenInclude(R => R.Ticket)
                .ThenInclude(S => S.Category)
                .Where(Q => Q.Id == bookedTicketId)
                .FirstOrDefaultAsync();

            if (bookedTicket == null)
            {
                throw new KeyNotFoundException($"Booking {bookedTicketId} tidak ditemukan");
            }

            var detail = bookedTicket.Details
                .Where(Q => Q.TicketCode == ticketCode.ToUpper())
                .FirstOrDefault();

            if (detail == null)
            {
                throw new KeyNotFoundException($"Kode tiket {ticketCode.ToUpper()} tidak ditemukan");
            }

            if (quantity > detail.Quantity)
            {
                throw new ArgumentException($"Jumlah tiket yang dimasukkan melebihi jumlah tiket yang dipesan sebelumnya ({detail.Quantity})");
            }

            detail.Quantity -= quantity;

            await _db.SaveChangesAsync();

            if (detail.Quantity == 0)
            {
                _db.Remove(detail);
                await _db.SaveChangesAsync();
            }

            if (!bookedTicket.Details.Any())
            {
                _db.Remove(bookedTicket);
                await _db.SaveChangesAsync();
            }

            var response = bookedTicket.Details
                .Select(Q => new ModifyTicketResponse
                {
                    TicketCode = Q.TicketCode,
                    TicketName = Q.Ticket.Name,
                    Quantity = Q.Quantity,
                    CategoryName = Q.Ticket.Category.Name
                }).ToList();

            return response;
        }
    }
}
