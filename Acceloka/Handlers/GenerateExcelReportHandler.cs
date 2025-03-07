using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Repositories;
using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Acceloka.Handlers
{
    public class GenerateExcelReportHandler : IRequestHandler<GenerateExcelReportCommand, byte[]>
    {
        private readonly BookedTicketRepository _repo;

        public GenerateExcelReportHandler(BookedTicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<byte[]> Handle(GenerateExcelReportCommand request, CancellationToken cancellationToken)
        {
            var bookedTicket = await _repo.GetBookedTicketByIdAsync(request.BookedTicketId, cancellationToken);

            var dataTable = new DataTable("BookedTickets");

            dataTable.Columns.Add("Booked Ticket ID", typeof(int));
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Category", typeof(string));
            dataTable.Columns.Add("Ticket Code", typeof(string));
            dataTable.Columns.Add("Ticket Name", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("Price", typeof(int));
            dataTable.Columns.Add("Total Price", typeof(int));

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

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(dataTable);
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
