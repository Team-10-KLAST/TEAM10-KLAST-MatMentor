using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Exceptions;
using Application.Features.ChatTutor.AskTutor;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.AI;

public sealed class OllamaTutorModelClient : ITutorModelClient
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public OllamaTutorModelClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _model = configuration["Ollama:Model"] ?? "matmentor-tutor:latest";
    }

    // Calls Ollama's non-streaming chat API with the trusted tutor policy and transient history.
    public async Task<string> AskAsync(AskTutorRequest request, CancellationToken cancellationToken)
    {
        var messages = new List<OllamaMessage>
        {
            new("system", TutorInstructions.SystemPrompt),
            new("user", $"Opgavetekst:\n{request.ExerciseText}\n\nDet har eleven prøvet:\n{request.AttemptText}")
        };

        messages.AddRange((request.Messages ?? []).Select(message =>
            new OllamaMessage(message.Role!, message.Content!)));

        var payload = new OllamaRequest(_model, false, messages);
        try
        {
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "api/chat", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new TutorUnavailableException("The AI tutor is temporarily unavailable.");

            OllamaResponse? result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
                cancellationToken: cancellationToken);

            return result?.Message?.Content
                ?? throw new TutorUnavailableException("The AI tutor returned an empty response.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TutorTimeoutException("The AI tutor took too long to respond.");
        }
        catch (HttpRequestException)
        {
            throw new TutorUnavailableException("The AI tutor is temporarily unavailable.");
        }
        catch (JsonException)
        {
            throw new TutorUnavailableException("The AI tutor returned an invalid response.");
        }
    }

    private sealed record OllamaMessage(string Role, string Content);

    private sealed record OllamaRequest(string Model, bool Stream, List<OllamaMessage> Messages);

    private sealed record OllamaResponse(OllamaReply? Message);

    private sealed record OllamaReply(string? Content);
}
