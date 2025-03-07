using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Models.Responses;
using Acceloka.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Handlers
{
    public class GetBookedTicketsHandler : IRequestHandler<GetBookedTicketsCommand, List<GetBookedTicketsResponse>>
    {
        private readonly BookedTicketRepository _repo;

        public GetBookedTicketsHandler(BookedTicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<GetBookedTicketsResponse>> Handle(GetBookedTicketsCommand request, CancellationToken cancellationToken)
        {
            var bookedTicket = await _repo.GetBookedTicketByIdAsync(request.BookedTicketId, cancellationToken);

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
    }
}
