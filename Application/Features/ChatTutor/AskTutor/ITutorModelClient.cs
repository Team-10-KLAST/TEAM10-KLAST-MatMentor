namespace Application.Features.ChatTutor.AskTutor;

public interface ITutorModelClient
{
    Task<string> AskAsync(AskTutorRequest request, CancellationToken cancellationToken);
}
