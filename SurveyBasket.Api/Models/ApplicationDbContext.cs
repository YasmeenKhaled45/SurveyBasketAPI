using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.Api.Models
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor contextAccessor): 
        IdentityDbContext<ApplicationUser,ApplicationRole,string>(options)
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public DbSet<Poll> Polls { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteAnswer> VoteAnswers { get; set; }
        public DbSet<Answer> Answers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            var cascadeFK = modelBuilder.Model.
                GetEntityTypes().SelectMany(t => t.GetForeignKeys())
                .Where(t => t.DeleteBehavior == DeleteBehavior.Cascade && !t.IsOwnership);
            foreach(var t in cascadeFK)
            {
                t.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();   // return entries that enhirit from AuditableEntity
            foreach (var entry in entries)
            {
                var userId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                if(entry.State == EntityState.Added)
                {
                    entry.Property(x => x.CreateById).CurrentValue = userId;
                }
                else if(entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.UpdatedById).CurrentValue = userId;
                    entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
