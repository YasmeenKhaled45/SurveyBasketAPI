using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Contracts.Results;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class ResultService(ApplicationDbContext context): IResultService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<PollVotesResponses>> GetPollVotes(int PollId,CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls.Where(x=>x.Id == PollId).Select(x => new PollVotesResponses
            (
                x.Title,
                x.Votes.Select(v=> new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.Answers.Select(a=> new QuestionAnswerResponse(
                        a.Question.Content,
                        a.Answer.Content
                      ))
                  ))
            )).SingleOrDefaultAsync(cancellationToken);
            return poll is null ? Result.Failure<PollVotesResponses>(new Error("PollErrors","Poll Not Found"))
                : Result.Success(poll) ;
        }

        public async Task<Result<IEnumerable<VotesPerDayResponse>>> VotesPerDay(int PollId , CancellationToken cancellationToken =default)
        {
             var pollexists = await _context.Polls.AnyAsync(x => x.Id == PollId,cancellationToken:cancellationToken);
            if (!pollexists)
                return Result.Failure<IEnumerable<VotesPerDayResponse>>(new Error("PollErrors", "Poll Not Found!"));
            var votes = await _context.Votes
               .Where(x => x.PollId == PollId)
                 .ToListAsync(cancellationToken); 

            var votesPerDay = votes
                .GroupBy(x => DateOnly.FromDateTime(x.SubmittedOn)) 
                .Select(g => new VotesPerDayResponse(
                    g.Key,
                    g.Count() 
                ))
                .ToList();
            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
        }
        public async Task<Result<IEnumerable<VotesPerQuestion>>> VotesPerQuestion(int PollId, CancellationToken cancellationToken = default)
        {
            var pollexists = await _context.Polls.AnyAsync(x => x.Id == PollId);
            if (!pollexists)
                return Result.Failure<IEnumerable<VotesPerQuestion>>(new Error("PollErrors", "Poll Not Found!"));

            var votesperquestion = await _context.VoteAnswers.Where(x => x.Vote.PollId == PollId)
                .Select(x => new VotesPerQuestion
                (
                    x.Question.Content,
                    x.Question.Votes.GroupBy(x => new { answers = x.Answer.Id, answercontent = x.Answer.Content })
                    .Select(g => new VotesPerAnswer
                     (
                        g.Key.answercontent,
                        g.Count()
                     ))
                )).ToListAsync (cancellationToken);
            return Result.Success<IEnumerable<VotesPerQuestion>>(votesperquestion);
                
        }
    }
}
