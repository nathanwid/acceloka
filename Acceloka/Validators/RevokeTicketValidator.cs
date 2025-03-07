using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Validators
{
    public class RevokeTicketValidator : AbstractValidator<RevokeTicketCommand>
    {
        private readonly BookedTicketRepository _bookedTicketRepo;

        public RevokeTicketValidator(BookedTicketRepository bookedTicketRepo)
        {
            _bookedTicketRepo = bookedTicketRepo;

            RuleFor(Q => Q.BookedTicketId)
                .MustAsync(async (id, cancellation) => 
                    await _bookedTicketRepo.BookedTicketExixtsAsync(id, cancellation))
                .WithMessage(Q => $"BookedTicketId {Q.BookedTicketId} not found")
                .DependentRules(() =>
                {
                    RuleFor(Q => Q.TicketCode)
                        .NotEmpty().WithMessage("TicketCode cannot be empty")
                        .MustAsync(async (request, code, cancellation) =>
                            await _bookedTicketRepo.TicketExistsInBookingAsync(request.BookedTicketId, code, cancellation))
                        .WithMessage(Q => $"Ticket {Q.TicketCode.ToUpper()} not found in booking")
                        .DependentRules(() =>
                        {
                            RuleFor(Q => Q.Quantity)
                                .GreaterThan(0).WithMessage("Quantity must be at least 1")
                                .MustAsync(async (request, quantity, context, cancellation) =>
                                {
                                    var bookedQuantity = await _bookedTicketRepo.GetBookedTicketQuantityAsync(request.BookedTicketId, request.TicketCode, cancellation);
                                    context.MessageFormatter.AppendArgument("BookedQuantity", bookedQuantity);
                                    return quantity <= bookedQuantity;
                                })
                                .WithMessage("Quantity entered exceeds the previously booked ({BookedQuantity})");
                        });

                });
        }
    }
}
