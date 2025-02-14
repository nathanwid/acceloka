using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Services
{
    public class TicketService
    {
        private readonly AccelokaContext _db;

        public TicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<(List<GetAvailableTicketsResponse>, int totalTickets)> GetAvailableTickets(
            string? categoryName, string? ticketCode, string? ticketName, int? maxPrice,
            DateTime? minEventDate, DateTime? maxEventDate, string? orderBy, string? orderState,
            int page, int pageSize)
        {
            var query = _db.Tickets.Where(Q => Q.Quota > 0).AsQueryable();

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(Q => Q.Category.Name.Contains(categoryName));
            }

            if (!string.IsNullOrEmpty(ticketCode))
            {
                query = query.Where(Q => Q.Code.Contains(ticketCode));
            }

            if (!string.IsNullOrEmpty(ticketName))
            {
                query = query.Where(Q => Q.Name.Contains(ticketName));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(Q => Q.Price <= maxPrice);
            }

            if (minEventDate.HasValue)
            {
                query = query.Where(Q => Q.Date >= minEventDate);
            }

            if (maxEventDate.HasValue)
            {
                query = query.Where(Q => Q.Date <= maxEventDate);
            }

            orderBy = string.IsNullOrEmpty(orderBy) ? "ticketCode" : orderBy;
            orderState = string.IsNullOrEmpty(orderState) ? "asc" : orderState.ToLower();

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
                }).ToListAsync();

            var totalTickets = await query.CountAsync();

            return (tickets, totalTickets);
        }
    }
}
