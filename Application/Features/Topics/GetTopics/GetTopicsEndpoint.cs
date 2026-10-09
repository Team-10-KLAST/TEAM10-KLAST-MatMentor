using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Topics.GetTopics;

public static class GetTopicsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topics", async (GetTopicsHandler handler) =>
        {
            var response = await handler.HandleAsync();
            return Results.Ok(response);
        });
    }
}