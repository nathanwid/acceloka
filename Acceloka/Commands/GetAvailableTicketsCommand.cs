using Acceloka.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Acceloka.Commands
{
    public record GetAvailableTicketsCommand(
        string? CategoryName,
        string? TicketCode,
        string? TicketName,
        int? MaxPrice,
        DateTime? MinEventDate,
        DateTime? MaxEventDate,
        string? OrderBy,
        string? OrderState,
        int? Page) : IRequest<(List<GetAvailableTicketsResponse>, int)>;
}
