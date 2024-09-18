using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Models;
using System.Linq.Dynamic.Core;
namespace SurveyBasket.Api.Services
{
    public class QuestionService(ApplicationDbContext context) : IQuestionService
    {
        private readonly ApplicationDbContext Context = context;

        public async Task<Result<PaginatedList<QuestionResponse>>> GetAllQuestions(int PollId, RequestFilters filters,CancellationToken cancellationToken)
        {
            var pollexists = await Context.Polls.AnyAsync(x => x.Id == PollId);
            if (!pollexists)
                return Result.Failure<PaginatedList<QuestionResponse>>(new Error("AddQuestionFailed", "Poll Not Found!"));
            var query = Context.Questions.Where(x => x.PollId == PollId);
            if (!string.IsNullOrEmpty(filters.SortColumn))
            {
                query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");
            }
            var source = query.Include(x => x.Answers).ProjectToType<QuestionResponse>()
                .AsNoTracking();
            var questions = await PaginatedList<QuestionResponse>.Create(source, filters.PageNumber, filters.PageSize, cancellationToken);
            return Result.Success(questions);
        }
        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableQuestions(int PollId, string UserId, CancellationToken cancellationToken)
        {
            var hasvote = await Context.Votes.AnyAsync(x => x.PollId == PollId && x.UserId == UserId, cancellationToken);
            if (hasvote)
                return Result.Failure<IEnumerable<QuestionResponse>>(new Error("DuplicatedVote","This User already voted before!"));
            var pollexists = await Context.Polls.AnyAsync(x => x.Id == PollId && x.Ispublished == true && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow),cancellationToken);
            if (!pollexists)
                return Result.Failure<IEnumerable<QuestionResponse>>(new Error("PollErrors", "Poll Not Found!"));
            var questions = await Context.Questions.Where(x => x.PollId == PollId && x.IsActive)
                .Include(x => x.Answers).Select(q => new QuestionResponse
                (
                    q.Id,
                    q.Content,
                    q.Answers.Where(a=>a.IsActive).Select(a=>new AnswerResponse(a.Id, a.Content))
                )).AsNoTracking().ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }
        public async Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest question, CancellationToken cancellationToken)
        {
            var pollexists = await Context.Polls.AnyAsync(x => x.Id == PollId);
            if (!pollexists)
                return Result.Failure<QuestionResponse>(new Error("AddQuestionFailed" , "Poll Not Found!"));

            var questionexists = await Context.Questions.AnyAsync(x=>x.Content == question.Content && x.Id == PollId,cancellationToken);
            if(questionexists)
                return Result.Failure<QuestionResponse>(new Error("AddQuestionFailed", "Another Question with same content"));
            var questionn = question.Adapt<Question>();
            questionn.PollId  = PollId;
            await Context.Questions.AddAsync(questionn, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Success(questionn.Adapt<QuestionResponse>());
        }

        public async Task<Result<QuestionResponse>> GetQuestionById(int PollId, int Id, CancellationToken cancellationToken)
        {
            var question = await Context.Questions.Where(x => x.Id == Id && x.PollId == PollId)
                .Include(x => x.Answers).ProjectToType<QuestionResponse>().AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);
            if (question is null)
                return Result.Failure<QuestionResponse>(new Error("Failed to load question", "Question Not Found!"));

            return Result.Success(question.Adapt<QuestionResponse>());  
        }

        public async Task<Result> ToggleQuestion(int PollId, int Id, CancellationToken cancellationToken)
        {
            var question = await Context.Questions.SingleOrDefaultAsync(x => x.Id == Id && x.PollId == PollId, cancellationToken);
            if (question is null)
                return Result.Failure(new Error("Failed to toggle question", "Question Not Found!"));
            question.IsActive = !question.IsActive;
            await Context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(int PollId, int Id, QuestionRequest request, CancellationToken cancellationToken)
        {
           var questioncontent = await Context.Questions.AnyAsync(x=>x.PollId == PollId && 
           x.Id != Id
           && x.Content == request.Content, cancellationToken);
            if (questioncontent)
                return Result.Failure(new Error("Failed to update question", "Duplicate Questions Content!"));

            var question = await Context.Questions.Include(x => x.Answers)
                .SingleOrDefaultAsync(x => x.PollId == PollId && x.Id == Id, cancellationToken);
            if(question is null)
                return Result.Failure(new Error("Failed to update question", "Question Not Found!"));
            question.Content = request.Content;
            var currentanswers = question.Answers.Select(x=>x.Content).ToList();
            var newanswers = request.Answers.Except(currentanswers).ToList();
            newanswers.ForEach(answer =>
            {
                question.Answers.Add(new Answer { Content = answer });
            });
            question.Answers.ToList().ForEach(answer =>
            {
                answer.IsActive = request.Answers.Contains(answer.Content);
            });
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
