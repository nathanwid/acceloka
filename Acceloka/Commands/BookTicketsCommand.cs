using Acceloka.Models.Requests;
using Acceloka.Models.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Acceloka.Commands
{
    public record BookTicketsCommand(
        DateTime BookingDate,
        List<DetailModel> Tickets) : IRequest<BookTicketsResponse>;
}
