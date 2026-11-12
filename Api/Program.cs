using Api.Middleware;
using Api.OpenApi;
using Application;
using Application.Features.Auth.Login;
using Application.Features.ChatTutor.Conversations;
using Application.Features.ExerciseSets.CompleteExerciseSetAttempt;
using Application.Features.ExerciseSets.StartExerciseSet;
using Application.Features.ExerciseSets.SubmitAnswer;
using Application.Features.Students.CreateStudentProfile;
using Application.Features.Topics.GetTopics;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);
// True when the API is only started to generate the OpenAPI document at build time
bool isGeneratingOpenApi = Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("TutorRateLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.User.FindFirst("StudentId")?.Value
                ?? context.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.AddPolicy("TutorReadRateLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.User.FindFirst("StudentId")?.Value
                ?? context.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Du har sendt for mange beskeder. Vent et øjeblik og prøv igen." },
            cancellationToken);
    };
});
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<BearerSecurityRequirementTransformer>();
});

IConfiguration jwtSettings = builder.Configuration.GetSection("JwtSettings");
string jwtSecretKey = jwtSettings["SecretKey"]
    ?? (isGeneratingOpenApi
    ? "placeholder-key-only-used-when-generating-openapi-document"
    : throw new InvalidOperationException("JwtSettings:SecretKey is not configured"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


var app = builder.Build();

if (!isGeneratingOpenApi)
{
    using var scope = app.Services.CreateScope();
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "MatMentor API v1"));
    Api.Development.FakeOllamaEndpoints.Map(app);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

CreateStudentProfileEndpoint.Map(app);
GetTopicsEndpoint.Map(app);
StartExerciseSetEndpoint.Map(app);
SubmitAnswerEndpoint.Map(app);
CompleteExerciseSetAttemptEndpoint.Map(app);
LoginEndpoint.Map(app);
TutorConversationEndpoints.Map(app);


app.Run();