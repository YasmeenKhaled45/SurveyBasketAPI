using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services
{
    public interface IQuestionService
    {
        Task<Result<PaginatedList<QuestionResponse>>> GetAllQuestions(int PollId,RequestFilters filters,CancellationToken cancellationToken);
        Task<Result<IEnumerable<QuestionResponse>>> GetAvailableQuestions(int PollId, string UserId, CancellationToken cancellationToken);
        Task<Result<QuestionResponse>> GetQuestionById(int PollId, int Id, CancellationToken cancellationToken);
        Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest question,CancellationToken cancellationToken);
        Task<Result> ToggleQuestion(int PollId,int Id, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(int PollId, int Id,QuestionRequest request, CancellationToken cancellationToken);
    }
}
