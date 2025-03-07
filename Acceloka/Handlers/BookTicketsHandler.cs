using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Models.Responses;
using Acceloka.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Handlers
{
    public class BookTicketsHandler : IRequestHandler<BookTicketsCommand, BookTicketsResponse>
    {
        private readonly BookedTicketRepository _bookedTicketRepo;
        private readonly TicketRepository _ticketRepo;

        public BookTicketsHandler(BookedTicketRepository bookedTicketRepo, TicketRepository ticketRepo)
        {
            _bookedTicketRepo = bookedTicketRepo;
            _ticketRepo = ticketRepo;
        }

        public async Task<BookTicketsResponse> Handle(BookTicketsCommand request, CancellationToken cancellationToken)
        {
            var ticketCodes = request.Tickets.Select(Q => Q.TicketCode).ToList();

            var tickets = await _ticketRepo.GetTicketsByCodesAsync(ticketCodes, cancellationToken);

            var details = new List<Detail>();

            var totalPrice = 0;

            foreach (var detail in request.Tickets)
            {
                var ticket = tickets.First(Q => Q.Code == detail.TicketCode.ToUpper());

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

            _bookedTicketRepo.Add(bookedTicket);
            await _bookedTicketRepo.SaveChangesAsync(cancellationToken);

            foreach (var detail in details)
            {
                detail.BookedTicketId = bookedTicket.Id;
            }

            _bookedTicketRepo.AddRange(details);
            await _bookedTicketRepo.SaveChangesAsync(cancellationToken);

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
    }
}
