using Acceloka.Models.Responses;
using MediatR;

namespace Acceloka.Commands
{
    public record GetBookedTicketsCommand(int BookedTicketId) : IRequest<List<GetBookedTicketsResponse>>;
}
