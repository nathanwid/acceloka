using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Models.Responses;
using Acceloka.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Handlers
{
    public class RevokeTicketHandler : IRequestHandler<RevokeTicketCommand, List<ModifyTicketResponse>>
    {
        private readonly BookedTicketRepository _repo;

        public RevokeTicketHandler(BookedTicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ModifyTicketResponse>> Handle(RevokeTicketCommand request, CancellationToken cancellationToken)
        {
            var bookedTicket = await _repo.GetBookedTicketByIdAsync(request.BookedTicketId, cancellationToken);

            var detail = bookedTicket.Details
                .Where(Q => Q.TicketCode == request.TicketCode.ToUpper())
                .First();

            detail.Quantity -= request.Quantity;

            await _repo.SaveChangesAsync(cancellationToken);

            if (detail.Quantity == 0)
            {
                _repo.Remove(detail);
                await _repo.SaveChangesAsync(cancellationToken);
            }

            if (!bookedTicket.Details.Any())
            {
                _repo.Remove(bookedTicket);
                await _repo.SaveChangesAsync(cancellationToken);
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
