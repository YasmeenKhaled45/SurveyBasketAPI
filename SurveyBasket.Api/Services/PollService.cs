using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class PollService(ApplicationDbContext context , INotficationService notficationService) : IPollService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly INotficationService notficationService = notficationService;

        public async Task<IEnumerable<PollResponse>> GetAll(CancellationToken cancellationToken = default) => 
            await _context.Polls.AsNoTracking().ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);
        public async Task<IEnumerable<PollResponse>> GetAvailablePolls(CancellationToken cancellationToken = default)=>
            await _context.Polls.Where(x=>x.Ispublished == true && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)).AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);
        public async Task<Result<PollResponse>> Add(PollRequest poll, CancellationToken cancellation = default)
        {
            var existingPoll = await _context.Polls.AnyAsync(x=>x.Title == poll.Title,cancellation);
            if (existingPoll)
                return Result.Failure<PollResponse>(new Error("Duplicated Poll Title","Another poll with same title exists"));
            var pollEntity = poll.Adapt<Poll>();
            await _context.Polls.AddAsync(pollEntity, cancellation);
            await _context.SaveChangesAsync(cancellation);
            return Result.Success(pollEntity.Adapt<PollResponse>());
        }
        public async Task<Result> Update(int Id, PollRequest poll,CancellationToken cancellationToken = default)
        {
            var existingPoll = await _context.Polls.AnyAsync(x => x.Title == poll.Title && x.Id != Id, cancellationToken);
            if (existingPoll)
                return Result.Failure(new Error("Duplicated Poll Title", "Another poll with same title exists"));
            var currentpoll = await _context.Polls.SingleOrDefaultAsync(s => s.Id == Id);
            if (currentpoll == null)
                return Result.Failure(new Error("Update Poll Failed","Poll Not Found"));

            currentpoll.Title = poll.Title;
            currentpoll.Description = poll.Description;
            currentpoll.StartsAt = poll.StartsAt;
            currentpoll.EndsAt = poll.EndsAt;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Delete(int Id,CancellationToken cancellationToken=default)
        {
            var currentpoll = await _context.Polls.SingleOrDefaultAsync(s => s.Id == Id);
            if (currentpoll == null)
                return Result.Failure(new Error("Delete Poll Failed!" , "Poll Not Found"));

            _context.Polls.Remove(currentpoll);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> TogglePublishStatus(int Id, CancellationToken cancellationToken = default)
        {
            var currentpoll = await _context.Polls.SingleOrDefaultAsync(s => s.Id == Id);
            if (currentpoll == null)
                return Result.Failure(new Error("Toggle Poll Failed!","Poll Not Found"));

            currentpoll.Ispublished = !currentpoll.Ispublished;
            await _context.SaveChangesAsync(cancellationToken);

            if (currentpoll.Ispublished && currentpoll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                BackgroundJob.Enqueue(()=> notficationService.SendPollNotfication(currentpoll.Id));
            return Result.Success();
        }
    }
}
