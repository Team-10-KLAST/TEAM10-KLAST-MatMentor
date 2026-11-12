using Application.Common.Exceptions;
using Application.Features.ChatTutor.AskTutor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.AI;

public sealed class OllamaTutorModelClient : ITutorModelClient
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<OllamaTutorModelClient> _logger;

    public OllamaTutorModelClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OllamaTutorModelClient> logger)
    {
        _httpClient = httpClient;
        _model = configuration["Ollama:Model"] ?? "matmentor-tutor:latest";
        _logger = logger;
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
                throw new ServiceUnavailableException("The AI tutor is temporarily unavailable.");

            OllamaResponse? result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
                cancellationToken: cancellationToken);

            return result?.Message?.Content
                ?? throw new ServiceUnavailableException("The AI tutor returned an empty response.");
        }
        catch (BrokenCircuitException)
        {
            //The circuit is open, so nothing was sent. The tutor is known to be failing.
            _logger.LogWarning("Tutor request rejected: the circuit is open.");
            throw new ServiceUnavailableException("The AI tutor is temporarily unavailable.");
        }
        catch (TimeoutRejectedException)
        {
            //The whole pipeline ran out of time, including the retry.
            _logger.LogWarning("Tutor request exceeded the total time budget.");
            throw new ServiceTimeoutException("The AI tutor took too long to respond.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            //The student did not cancel, so this is the tutor taking too long.
            _logger.LogWarning("Tutor request timed out.");
            throw new ServiceTimeoutException("The AI tutor took too long to respond.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Tutor request failed.");
            throw new ServiceUnavailableException("The AI tutor is temporarily unavailable.");
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Tutor returned a response that could not be read.");
            throw new ServiceUnavailableException("The AI tutor returned an invalid response.");
        }
    }

    private sealed record OllamaMessage(string Role, string Content);

    private sealed record OllamaRequest(string Model, bool Stream, List<OllamaMessage> Messages);

    private sealed record OllamaResponse(OllamaReply? Message);

    private sealed record OllamaReply(string? Content);
}
