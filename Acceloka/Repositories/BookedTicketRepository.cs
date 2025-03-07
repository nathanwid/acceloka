using Acceloka.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Acceloka.Repositories
{
    public class BookedTicketRepository
    {
        private readonly AccelokaContext _db;

        public BookedTicketRepository(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<BookedTicket> GetBookedTicketByIdAsync(int bookedTicketId, CancellationToken cancellationToken)
        {
            return await _db.BookedTickets
                .Include(Q => Q.Details)
                .ThenInclude(R => R.Ticket)
                .ThenInclude(S => S.Category)
                .FirstAsync(Q => Q.Id == bookedTicketId, cancellationToken);
        }

        public async Task<List<BookedTicket>> GetAllBookedTicketsAsync(CancellationToken cancellationToken)
        {
            return await _db.BookedTickets
                .Include(Q => Q.Details)
                .ThenInclude(R => R.Ticket)
                .ThenInclude(S => S.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> BookedTicketExixtsAsync(int bookedTicketId, CancellationToken cancellationToken)
        {
            return await _db.BookedTickets.AnyAsync(Q => Q.Id == bookedTicketId, cancellationToken);
        }

        public async Task<bool> TicketExistsInBookingAsync(int bookedTicketId, string ticketCode, CancellationToken cancellationToken)
        {
            return await _db.BookedTickets
                .Include(Q => Q.Details)
                .Where(Q => Q.Id == bookedTicketId)
                .SelectMany(Q => Q.Details)
                .Where(R => R.TicketCode == ticketCode.ToUpper())
                .AnyAsync(cancellationToken);
        }

        public async Task<int> GetBookedTicketQuantityAsync(int bookedTicketId, string ticketCode, CancellationToken cancellationToken)
        {
            var detail = await _db.BookedTickets
                .Include(Q => Q.Details)
                .Where(Q => Q.Id == bookedTicketId)
                .SelectMany(Q => Q.Details)
                .Where(R => R.TicketCode == ticketCode.ToUpper())
                .FirstOrDefaultAsync(cancellationToken);

            return detail?.Quantity ?? 0;
        }

        public async Task<bool> HasAnyBookedTicketsAsync(CancellationToken cancellationToken)
        {
            return await _db.BookedTickets.AnyAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        public void Remove<T>(T entity) where T : class
        {
            _db.Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _db.Update(entity);
        }

        public void Add<T>(T entity) where T : class
        {
            _db.Add(entity);
        }

        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            _db.AddRange(entities);
        }
    }
}
