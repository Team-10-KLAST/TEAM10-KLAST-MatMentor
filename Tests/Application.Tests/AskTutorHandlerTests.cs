using Application.Common.Exceptions;
using Application.Features.ChatTutor.AskTutor;
using Xunit;

namespace Application.Tests;

public sealed class AskTutorHandlerTests
{
    [Fact]
    public async Task HandleAsync_SendsFullRequestAndReturnsReply()
    {
        var model = new FakeTutorModelClient("  Hvad har du forsøgt?  ");
        var handler = new AskTutorHandler(model);
        var request = new AskTutorRequest("Opgave", "Et forsøg", []);

        AskTutorResponse result = await handler.HandleAsync(request, CancellationToken.None);

        Assert.Same(request, model.LastRequest);
        Assert.Equal("Hvad har du forsøgt?", result.Reply);
        Assert.Equal(1, model.CallCount);
    }

    [Fact]
    public async Task HandleAsync_EmptyModelReply_IsRejected()
    {
        var handler = new AskTutorHandler(new FakeTutorModelClient("  "));
        await Assert.ThrowsAsync<TutorUnavailableException>(() =>
            handler.HandleAsync(new AskTutorRequest("Opgave", "Forsøg", []), CancellationToken.None));
    }

    private sealed class FakeTutorModelClient : ITutorModelClient
    {
        private readonly string _reply;
        public AskTutorRequest? LastRequest { get; private set; }
        public int CallCount { get; private set; }

        public FakeTutorModelClient(string reply)
        {
            _reply = reply;
        }

        public Task<string> AskAsync(AskTutorRequest request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            CallCount++;
            return Task.FromResult(_reply);
        }
    }
}
