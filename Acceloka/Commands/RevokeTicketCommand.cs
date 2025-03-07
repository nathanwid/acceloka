using Acceloka.Models.Responses;
using MediatR;

namespace Acceloka.Commands
{
    public record RevokeTicketCommand(
        int BookedTicketId,
        string TicketCode,
        int Quantity) : IRequest<List<ModifyTicketResponse>>;
}
