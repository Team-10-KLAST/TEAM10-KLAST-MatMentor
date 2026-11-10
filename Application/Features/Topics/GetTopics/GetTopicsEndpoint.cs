using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Topics.GetTopics;

public static class GetTopicsEndpoint
{
    // Maps GET /api/topics, which returns all topics the student can practise
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topics", async (GetTopicsHandler handler) =>
        {
            GetTopicsResponse response = await handler.HandleAsync();
            return TypedResults.Ok(response);
        })
        .WithTags("Topics")
        .Produces<GetTopicsResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}