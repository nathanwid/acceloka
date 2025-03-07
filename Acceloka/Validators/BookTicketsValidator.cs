using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Validators
{
    public class BookTicketsValidator : AbstractValidator<BookTicketsCommand>
    {
        private readonly TicketRepository _ticketRepo;

        public BookTicketsValidator(TicketRepository ticketRepo)
        {
            _ticketRepo = ticketRepo;

            RuleForEach(Q => Q.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(Q => Q.TicketCode)
                    .NotEmpty().WithMessage("TicketCode cannot be empty")
                    .MustAsync(async (code, cancellation) => 
                        await _ticketRepo.TicketExistsAsync(code, cancellation))
                    .WithMessage(Q => $"Ticket {Q.TicketCode.ToUpper()} is not registered")
                    .DependentRules(() =>
                    {
                        ticket.RuleFor(Q => Q.TicketCode)
                            .MustAsync(async (code, cancellation) =>
                                await _ticketRepo.IsTicketAvailableAsync(code, cancellation))
                            .WithMessage(Q => $"Ticket {Q.TicketCode.ToUpper()} is out of quota");

                        ticket.RuleFor(Q => Q.Quantity)
                            .GreaterThan(0).WithMessage("Quantity must be at least 1")
                            .MustAsync(async (detail, quantity, cancellation) =>
                                await _ticketRepo.IsQuantityValidAsync(detail.TicketCode, quantity, cancellation))
                            .WithMessage(Q => $"Quantity entered for ticket {Q.TicketCode.ToUpper()} exceeds quota");

                        ticket.RuleFor(Q => Q.TicketCode)
                            .MustAsync(async (code, cancellation) =>
                                await _ticketRepo.IsEventDateValidAsync(code, cancellation))
                            .WithMessage(Q => $"The event for ticket {Q.TicketCode.ToUpper()} has passed");
                    });
            });
        }
    }
}
