using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Acceloka.Validators
{
    public class GenerateExcelReportValidator : AbstractValidator<GenerateExcelReportCommand>
    {
        private readonly BookedTicketRepository _bookedTicketRepo;

        public GenerateExcelReportValidator(BookedTicketRepository bookedTicketRepo)
        {
            _bookedTicketRepo = bookedTicketRepo;

            RuleFor(Q => Q.BookedTicketId)
                .MustAsync(async (id, cancellation) => 
                    await _bookedTicketRepo.BookedTicketExixtsAsync(id, cancellation))
                .WithMessage(Q => $"BookedTicketId {Q.BookedTicketId} not found");
        }
    }
}
