using Acceloka.Models.Requests;
using Acceloka.Models.Responses;
using MediatR;

namespace Acceloka.Commands
{
    public record EditBookedTicketCommand(
        int BookedTicketId,
        BookedTicketModel BookedTicket) : IRequest<List<ModifyTicketResponse>>;
}
