using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Validators
{
    public class EditBookedTicketValidator : AbstractValidator<EditBookedTicketCommand>
    {
        private readonly BookedTicketRepository _bookedTicketRepo;
        private readonly TicketRepository _ticketRepo;

        public EditBookedTicketValidator(BookedTicketRepository bookedTicketRepo, TicketRepository ticketRepo)
        {
            _bookedTicketRepo = bookedTicketRepo;
            _ticketRepo = ticketRepo;

            RuleFor(Q => Q.BookedTicketId)
                .MustAsync(async (id, cancellation) => 
                    await _bookedTicketRepo.BookedTicketExixtsAsync(id, cancellation))
                .WithMessage(Q => $"BookedTicketId {Q.BookedTicketId} not found")
                .DependentRules(() =>
                {
                    RuleForEach(Q => Q.BookedTicket.Tickets).ChildRules(ticket =>
                    {
                        ticket.RuleFor(Q => Q.TicketCode)
                            .NotEmpty().WithMessage("TicketCode cannot be empty")
                            .DependentRules(() =>
                            {
                                ticket.RuleFor(Q => Q.Quantity)
                                    .GreaterThan(0).WithMessage(Q => $"Quantity {Q.TicketCode} must be at least 1")
                                    .MustAsync(async (detail, quantity, context, cancellation) =>
                                    {
                                        var quota = await _ticketRepo.GetTicketQuotaAsync(detail.TicketCode, cancellation);
                                        context.MessageFormatter.AppendArgument("Quota", quota); 
                                        return quantity <= quota;
                                    })
                                    .WithMessage("Quantity entered exceeds quota ({Quota})");
                            });
                    });
                });
        }
    }

}
