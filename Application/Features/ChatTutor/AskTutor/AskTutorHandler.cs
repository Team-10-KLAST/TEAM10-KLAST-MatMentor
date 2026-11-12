using Application.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Features.ChatTutor.AskTutor;

public sealed class AskTutorHandler
{
    private readonly ITutorModelClient _modelClient;
    private readonly ILogger<AskTutorHandler> _logger;

    public AskTutorHandler(ITutorModelClient modelClient, ILogger<AskTutorHandler> logger)
    {
        _modelClient = modelClient;
        _logger = logger;
    }

    // Requests a guided tutor response using the supplied server-validated context.
    public async Task<AskTutorResponse> HandleAsync(
        AskTutorRequest request,
        CancellationToken cancellationToken)
    {
        string reply = await _modelClient.AskAsync(request, cancellationToken);
        if (string.IsNullOrWhiteSpace(reply))
        {
            _logger.LogWarning("Tutor returned an empty response.");
            throw new ServiceUnavailableException("The AI tutor did not return a usable response.");
        }

        if (reply.Trim().Length > Domain.TutorConversation.MaxMessageLength)
        {
            _logger.LogWarning("Tutor returned a response of {Length} characters.", reply.Trim().Length);
            throw new ServiceUnavailableException("The AI tutor returned a response that was too long.");
        }

        return new AskTutorResponse(reply.Trim());
    }
}
