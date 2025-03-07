using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Models.Responses;
using Acceloka.Repositories;
using Acceloka.Services;
using Acceloka.Validators;
using DocumentFormat.OpenXml.Drawing.Charts;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Handlers
{
    public class GetAvailableTicketsHandler : IRequestHandler<GetAvailableTicketsCommand, (List<GetAvailableTicketsResponse>, int)>
    {
        private readonly TicketRepository _repo;

        public GetAvailableTicketsHandler(TicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<(List<GetAvailableTicketsResponse>, int)> Handle(GetAvailableTicketsCommand request, CancellationToken cancellationToken)
        {
            return await _repo.GetAvailableTicketsAsync(request, cancellationToken);
        }
    }
}
