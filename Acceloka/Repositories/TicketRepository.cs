using Acceloka.Commands;
using Acceloka.Entities;
using Acceloka.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Acceloka.Repositories
{
    public class TicketRepository
    {
        private readonly AccelokaContext _db;

        public TicketRepository(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<(List<GetAvailableTicketsResponse>, int)> GetAvailableTicketsAsync(GetAvailableTicketsCommand request, CancellationToken cancellationToken)
        {
            var page = request.Page ?? 1;
            const int pageSize = 10;

            var query = _db.Tickets.Where(Q => Q.Quota > 0).AsQueryable();

            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                query = query.Where(Q => Q.Category.Name.Contains(request.CategoryName));
            }

            if (!string.IsNullOrEmpty(request.TicketCode))
            {
                query = query.Where(Q => Q.Code.Contains(request.TicketCode));
            }

            if (!string.IsNullOrEmpty(request.TicketName))
            {
                query = query.Where(Q => Q.Name.Contains(request.TicketName));
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(Q => Q.Price <= request.MaxPrice);
            }

            if (request.MinEventDate.HasValue)
            {
                query = query.Where(Q => Q.Date >= request.MinEventDate);
            }

            if (request.MaxEventDate.HasValue)
            {
                query = query.Where(Q => Q.Date <= request.MaxEventDate);
            }

            string orderBy = string.IsNullOrEmpty(request.OrderBy) ? "ticketCode" : request.OrderBy;
            string orderState = string.IsNullOrEmpty(request.OrderState) ? "asc" : request.OrderState.ToLower();

            query = orderBy switch
            {
                "categoryName" => orderState == "asc" ? query.OrderBy(Q => Q.Category.Name) : query.OrderByDescending(Q => Q.Category.Name),
                "ticketCode" => orderState == "asc" ? query.OrderBy(Q => Q.Code) : query.OrderByDescending(Q => Q.Code),
                "ticketName" => orderState == "asc" ? query.OrderBy(Q => Q.Name) : query.OrderByDescending(Q => Q.Name),
                "price" => orderState == "asc" ? query.OrderBy(Q => Q.Price) : query.OrderByDescending(Q => Q.Price),
                "eventDate" => orderState == "asc" ? query.OrderBy(Q => Q.Date) : query.OrderByDescending(Q => Q.Date),
                _ => query.OrderBy(Q => Q.Code)
            };

            var tickets = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(Q => new GetAvailableTicketsResponse
                {
                    EventDate = Q.Date.ToString("dd-MM-yyyy HH:mm"),
                    Quota = Q.Quota,
                    TicketCode = Q.Code,
                    TicketName = Q.Name,
                    CategoryName = Q.Category.Name,
                    Price = Q.Price
                }).ToListAsync(cancellationToken);

            var totalTickets = await query.CountAsync(cancellationToken);

            return (tickets, totalTickets);
        }

        public async Task<List<Ticket>> GetTicketsByCodesAsync(List<string> ticketCodes, CancellationToken cancellationToken)
        {
            return await _db.Tickets
                .Where(Q => ticketCodes.Contains(Q.Code))
                .Include(Q => Q.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTicketQuotaAsync(string ticketCode, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets.FirstAsync(Q => Q.Code == ticketCode, cancellationToken);
            return ticket.Quota;
        }

        public async Task<bool> TicketExistsAsync(string ticketCode, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(Q => Q.Code == ticketCode, cancellationToken);
            return ticket != null;
        }

        public async Task<bool> IsTicketAvailableAsync(string ticketCode, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets.FirstAsync(Q => Q.Code == ticketCode, cancellationToken);
            return ticket.Quota > 0;
        }

        public async Task<bool> IsEventDateValidAsync(string ticketCode, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(Q => Q.Code == ticketCode, cancellationToken);
            return ticket != null && DateTime.Now <= ticket.Date;
        }

        public async Task<bool> IsQuantityValidAsync(string ticketCode, int quantity, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(Q => Q.Code == ticketCode, cancellationToken);
            return ticket != null && quantity <= ticket.Quota;
        }
    }
}
