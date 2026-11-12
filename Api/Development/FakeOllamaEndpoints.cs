namespace Api.Development;

//A stand-in for Ollama, used to provoke failures on demand while testing resilience.
//Only mapped in Development.
public static class FakeOllamaEndpoints
{
    //The behaviour the next call should produce. Changed through the mode endpoint below.
    private static volatile string _mode = "ok";

    public static void Map(IEndpointRouteBuilder app)
    {
        //Switches behaviour while the API is running, so no restart is needed between tests.
        app.MapPut("/dev/fake-ollama/mode/{mode}", (string mode) =>
        {
            _mode = mode.ToLowerInvariant();
            return Results.Ok(new { mode = _mode });
        }).AllowAnonymous();

        app.MapPost("/dev/fake-ollama/api/chat", async (CancellationToken cancellationToken) =>
        {
            switch (_mode)
            {
                //The service hangs. The attempt timeout should cut this off.
                case "slow":
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                    return Results.Json(Reply("Et svar der kom for sent."));

                //The service is failing. Retry runs, then the circuit breaker takes over.
                case "error":
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);

                //The service answers, but not with what we expect.
                case "garbage":
                    return Results.Text("<html>ikke json</html>", "application/json");

                //The service answers with nothing usable.
                case "empty":
                    return Results.Json(Reply(""));

                default:
                    return Results.Json(Reply("Hvad tror du selv, det første skridt er?"));
            }
        }).AllowAnonymous();
    }

    private static object Reply(string content) => new { message = new { content } };
}