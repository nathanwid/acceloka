using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Models.Responses;
using Acceloka.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Handlers
{
    public class EditBookedTicketHandler : IRequestHandler<EditBookedTicketCommand, List<ModifyTicketResponse>>
    {
        private readonly BookedTicketRepository _repo;

        public EditBookedTicketHandler(BookedTicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ModifyTicketResponse>> Handle(EditBookedTicketCommand request, CancellationToken cancellationToken)
        {
            var bookedTicket = await _repo.GetBookedTicketByIdAsync(request.BookedTicketId, cancellationToken);

            foreach (var detailUpdate in request.BookedTicket.Tickets)
            {
                var detail = bookedTicket.Details
                    .Where(Q => Q.TicketCode == detailUpdate.TicketCode.ToUpper())
                    .First();

                int quantityChange = detailUpdate.Quantity - detail.Quantity;
                detail.Ticket.Quota -= quantityChange;

                detail.Quantity = detailUpdate.Quantity;

                _repo.Update(detail);
            }

            await _repo.SaveChangesAsync(cancellationToken);

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
