using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        public ApplicationDbContext Context { get; } = context;

        public async Task<Result> SaveVote(int PollID, string UserId, VoteRequest request, CancellationToken cancellationToken)
        {
            var hasvote = await Context.Votes.AnyAsync(x => x.PollId == PollID && x.UserId == UserId, cancellationToken);
            if (hasvote)
                return Result.Failure(new Error("DuplicatedVote", "This User already voted before!"));
            var pollexists = await Context.Polls.AnyAsync(x => x.Id == PollID && x.Ispublished == true && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
            if (!pollexists)
                return Result.Failure(new Error("PollErrors", "Poll Not Found!"));
            var availableQuestions = await Context.Questions.Where(x => x.PollId == PollID && x.IsActive)
                .Select(x => x.Id).ToListAsync(cancellationToken);
            if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(availableQuestions))
                return Result.Failure(new Error("VoteError","Invalid Questions"));

            var vote = new Vote
            {
               PollId = PollID,
               UserId = UserId,
               Answers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()

            };
            await Context.AddAsync(vote);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
