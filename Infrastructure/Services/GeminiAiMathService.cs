using Application.Features.ChatTutor.AskMathQuestion; // Updated namespace
using Google.GenAI;
using Google.GenAI.Types;

namespace Infrastructure.Services;

public class GeminiAiMathService : IAiMathService
{
    private readonly Client _geminiClient;

    public GeminiAiMathService(Client geminiClient)
    {
        _geminiClient = geminiClient;
    }


    public async Task<string> GetExplanationAsync(string question)
    {        
        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts = [new Part { Text = "Du er en hjælpsom matematiklærer. Led eleven på rette spor, men giv IKKE svaret. Svar på dansk." }]
            },
            Temperature = 0.4f
        };

        // Set a 10-second timeout to prevent hanging
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var response = await _geminiClient.Models.GenerateContentAsync(
        model: "gemini-3.5-flash-lite",
        contents: question,
        config: config,
        cancellationToken: cts.Token
        ).ConfigureAwait(false);

        return response.Text ?? "Jeg kunne ikke generere et svar lige nu.";
    }
}