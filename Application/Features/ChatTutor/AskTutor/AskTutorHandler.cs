using Application.Common.Exceptions;

namespace Application.Features.ChatTutor.AskTutor;

public sealed class AskTutorHandler
{
    private readonly ITutorModelClient _modelClient;

    public AskTutorHandler(ITutorModelClient modelClient)
    {
        _modelClient = modelClient;
    }

    // Requests a guided tutor response using the supplied server-validated context.
    public async Task<AskTutorResponse> HandleAsync(
        AskTutorRequest request,
        CancellationToken cancellationToken)
    {
        string reply = await _modelClient.AskAsync(request, cancellationToken);
        if (string.IsNullOrWhiteSpace(reply))
            throw new TutorUnavailableException("The AI tutor did not return a usable response.");

        if (reply.Trim().Length > Domain.TutorConversation.MaxMessageLength)
            throw new TutorUnavailableException("The AI tutor returned a response that was too long.");

        return new AskTutorResponse(reply.Trim());
    }
}
