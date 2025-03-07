using Acceloka.Commands;
using FluentValidation;

namespace Acceloka.Validators
{
    public class GetAvailableTicketsValidator : AbstractValidator<GetAvailableTicketsCommand>
    {
        public GetAvailableTicketsValidator()
        {
            RuleFor(Q => Q.Page)
                .Must(page => page == null || page > 0)
                .WithMessage("Page must be greater than 0 if provided");

            RuleFor(Q => Q.MaxPrice)
                .GreaterThan(0).WithMessage("MaxPrice must be greater than 0")
                .When(Q => Q.MaxPrice.HasValue);

            RuleFor(Q => Q.MinEventDate)
                .LessThanOrEqualTo(Q => Q.MaxEventDate).WithMessage("MinEventDate must be before or equal to MaxEventDate")
                .When(Q => Q.MinEventDate.HasValue && Q.MaxEventDate.HasValue);

            RuleFor(Q => Q.OrderBy)
                .Must(orderBy => new[] { "categoryName", "ticketCode", "ticketName", "price", "eventDate" }.Contains(orderBy))
                .WithMessage("Invalid OrderBy value")
                .When(Q => !string.IsNullOrEmpty(Q.OrderBy));

            RuleFor(Q => Q.OrderState)
                .Must(orderState => orderState?.ToLower() == "asc" || orderState?.ToLower() == "desc")
                .WithMessage("OrderState must be either 'asc' or 'desc'")
                .When(Q => !string.IsNullOrEmpty(Q.OrderState));
        }
    }
}
