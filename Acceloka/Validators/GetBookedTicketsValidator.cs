namespace Acceloka.Validators
{
    using Acceloka.Commands;
    using Acceloka.Entities;
    using Acceloka.Repositories;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class GetBookedTicketsValidator : AbstractValidator<GetBookedTicketsCommand>
    {
        private readonly BookedTicketRepository _bookedTicketRepo;

        public GetBookedTicketsValidator(BookedTicketRepository bookedTicketRepo)
        {
            _bookedTicketRepo = bookedTicketRepo;

            RuleFor(Q => Q.BookedTicketId)
                .GreaterThan(0).WithMessage("Invalid BookedTicketId")
                .DependentRules(() =>
                {
                    RuleFor(Q => Q.BookedTicketId)
                        .MustAsync(async (id, cancellation) => 
                            await _bookedTicketRepo.BookedTicketExixtsAsync(id, cancellation))
                        .WithMessage(Q => $"Booking {Q.BookedTicketId} not found");
                });
        }
    }

}
