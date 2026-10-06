using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.ExerciseSets.StartExerciseSet;

public static class StartExerciseSetEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/exercise-sets/start", async (
        StartExerciseSetRequest request,
        StartExerciseSetValidator validator,
        StartExerciseSetHandler handler) =>
        {
            var errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);
            
            var response = await handler.HandleAsync(request);
            return Results.Ok(response);
        
        });
    }
}
