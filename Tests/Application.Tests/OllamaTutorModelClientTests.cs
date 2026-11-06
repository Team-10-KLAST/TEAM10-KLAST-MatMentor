using System.Net;
using System.Text;
using System.Text.Json;
using Application.Features.ChatTutor.AskTutor;
using Infrastructure.AI;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Application.Tests;

public sealed class OllamaTutorModelClientTests
{
    [Fact]
    public async Task AskAsync_SendsTrustedPolicyInitialQuestionAndOrderedHistory()
    {
        var capture = new CaptureHandler();
        using var http = new HttpClient(capture) { BaseAddress = new Uri("http://localhost:11434/") };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["Ollama:Model"] = "matmentor-tutor:latest" }).Build();
        var client = new OllamaTutorModelClient(http, configuration);
        var request = new AskTutorRequest("Beregn rabat", "Jeg bruger procent", [
            new("assistant", "Hvilken pris starter du med?"),
            new("user", "200 kr")
        ]);

        string response = await client.AskAsync(request, CancellationToken.None);

        Assert.Equal("Hvilket tal er udgangspunktet?", response);
        Assert.Equal("/api/chat", capture.RequestPath);
        using var payload = JsonDocument.Parse(capture.RequestJson!);
        Assert.Equal("matmentor-tutor:latest", payload.RootElement.GetProperty("model").GetString());
        Assert.False(payload.RootElement.GetProperty("stream").GetBoolean());
        JsonElement messages = payload.RootElement.GetProperty("messages");
        Assert.Equal(4, messages.GetArrayLength());
        Assert.Equal("system", messages[0].GetProperty("role").GetString());
        Assert.Contains("LOVPORT", messages[0].GetProperty("content").GetString());
        Assert.Equal("user", messages[1].GetProperty("role").GetString());
        Assert.Contains("Beregn rabat", messages[1].GetProperty("content").GetString());
        Assert.Equal("assistant", messages[2].GetProperty("role").GetString());
        Assert.Equal("user", messages[3].GetProperty("role").GetString());
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public string? RequestJson { get; private set; }
        public string? RequestPath { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestPath = request.RequestUri?.AbsolutePath;
            RequestJson = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"message\":{\"content\":\"Hvilket tal er udgangspunktet?\"}}",
                    Encoding.UTF8, "application/json")
            };
        }
    }
}
