
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Helpers;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class NotficationService(ApplicationDbContext context,UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,IEmailSender emailSender) : INotficationService
    {
        private readonly ApplicationDbContext context = context;
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IHttpContextAccessor contextAccessor = contextAccessor;
        private readonly IEmailSender emailSender = emailSender;

        public async Task SendPollNotfication(int? pollId = null)
        {
            IEnumerable<Poll> polls = [];
            if (pollId.HasValue)
            {
                var poll = await context.Polls.SingleOrDefaultAsync(x => x.Id == pollId && x.Ispublished);
                polls = [poll!];
            }
            else
            {
                polls = await context.Polls.
                    Where(x=>x.Ispublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                    .AsNoTracking().ToListAsync();
            }
            var users = await userManager.GetUsersInRoleAsync(DefaultRoles.Member);
            var origin = contextAccessor.HttpContext?.Request.Headers.Origin;
            foreach (var poll in polls)
            {
                foreach (var user in users)
                {
                    var placeholders = new Dictionary<string, string>()
                    {
                        {"{{name}}", user.FirstName },
                        {"{{pollTill}}", poll.Title },
                        {"endDate",poll.EndsAt.ToString() },
                        {"url", $"{origin}/polls/SaveVote/{pollId}"}
                    };
                    var body = EmailBodyBuilder.GenerateEmailBody("PollNotfication", placeholders);
                    await emailSender.SendEmailAsync(user.Email!, $"SurveyBasket : New poll - {poll.Title}", body);
                }
            }
        }
    }
}
