using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Filters;
using SurveyBasket.Api.Contracts.JWT;
using SurveyBasket.Api.Models;
using SurveyBasket.Api.Services;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddHealthChecks();
builder.Services.AddRateLimiter(rateLimiterOptions =>
 {
     rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

     rateLimiterOptions.AddPolicy(RateLimiters.IpLimiter, httpContext =>
         RateLimitPartition.GetFixedWindowLimiter(
             partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
             factory: _ => new FixedWindowRateLimiterOptions
             {
                 PermitLimit = 2,
                 Window = TimeSpan.FromSeconds(20)
             }
     )
     );

     rateLimiterOptions.AddPolicy(RateLimiters.UserLimiter, httpContext =>
         RateLimitPartition.GetFixedWindowLimiter(
             partitionKey: httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
             factory: _ => new FixedWindowRateLimiterOptions
             {
                 PermitLimit = 2,
                 Window = TimeSpan.FromSeconds(20)
             }
     )
     );
     rateLimiterOptions.AddConcurrencyLimiter(RateLimiters.Concurrency, options =>
     {
         options.PermitLimit = 1000;
         options.QueueLimit = 100;
         options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
     });
 });
 builder.Services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionstring));
builder.Services.AddOptions<MailSettings>().BindConfiguration(nameof(MailSettings))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter Your Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
          new OpenApiSecurityScheme{
             Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              }

            },
            new List<string>()
          }
        });
});
//builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddOptions<JWTOptions>().BindConfiguration("JWT")
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
})  .AddJwtBearer(o=>
{
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:key"]!)),
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"]
    };
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
});
builder.Services.AddSingleton<IJWTProvider, JWTProvider>();
builder.Services.AddScoped<IPollService,PollService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IResultService,ResultService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IEmailSender,EmailService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IQuestionService,QuestionService>();
builder.Services.AddScoped<INotficationService,NotficationService>();
builder.Services.AddTransient<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddMapster();
var mappingconfig = TypeAdapterConfig.GlobalSettings;
mappingconfig.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton<IMapper>(new Mapper(mappingconfig));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseHangfireDashboard("/jobs",new DashboardOptions
{
    Authorization = [
        new HangfireCustomBasicAuthenticationFilter{
            User = app.Configuration.GetValue<string>("HangfireSettings:UserName"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "SurveyBasketJobs"
});
var factory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = factory.CreateScope();
var Notficationservices = scope.ServiceProvider.GetRequiredService<INotficationService>();
RecurringJob.AddOrUpdate("SendPollNotfications",()=> Notficationservices.SendPollNotfication(null),Cron.Daily);
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("health");
app.UseRateLimiter();
app.Run();
