using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class PollService(ApplicationDbContext context) : IPollService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Poll>> GetAll(CancellationToken cancellationToken = default) => 
            await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        public async Task<Poll> Add(Poll poll , CancellationToken cancellation)
        {
            await _context.Polls.AddAsync(poll , cancellation);
            await _context.SaveChangesAsync(cancellation);
            return poll;
        }

        public async Task<bool> Update(int Id, Poll poll,CancellationToken cancellationToken = default)
        {
            var currentpoll = await _context.Polls.SingleOrDefaultAsync(s => s.Id == Id);
            if (currentpoll == null)
                return false;

            currentpoll.Title = poll.Title;
            currentpoll.Description = poll.Description;
            currentpoll.StartsAt = poll.StartsAt;
            currentpoll.EndsAt = poll.EndsAt;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> Delete(int Id,CancellationToken cancellationToken=default)
        {
            var currentpoll = await _context.Polls.SingleOrDefaultAsync(s => s.Id == Id);
            if (currentpoll == null)
                return false;

            _context.Polls.Remove(currentpoll);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> TogglePublishStatus(int Id, CancellationToken cancellationToken = default)
        {
            var currentpoll = await _context.Polls.SingleOrDefaultAsync(s => s.Id == Id);
            if (currentpoll == null)
                return false;

            currentpoll.Ispublished = !currentpoll.Ispublished;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
