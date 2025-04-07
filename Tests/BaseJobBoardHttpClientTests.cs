using System.Net;
using Infrastructure.Services;
using Moq;
using Moq.Protected;

namespace Tests;

public class BaseJobBoardHttpClientTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly TestableBaseJobBoardHttpClient _testClient;

    // Testable derived class to test the abstract base class
    private class TestableBaseJobBoardHttpClient : BaseJobBoardHttpClient
    {
        public TestableBaseJobBoardHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }

        // Expose the protected method for testing
        public new Task<HttpContent> GetJobsAsync(Uri uri, bool usePost = false, HttpContent? requestContent = null)
        {
            return base.GetJobsAsync(uri, usePost, requestContent);
        }
    }

    public BaseJobBoardHttpClientTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_handlerMock.Object);
        _testClient = new TestableBaseJobBoardHttpClient(_httpClient);
    }

    [Fact]
    public async Task GetJobsAsync_GetRequest_Success_ReturnsContent()
    {
        // Arrange
        var expectedUri = new Uri("https://example.com/api/jobs");
        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("test content")
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        // Act
        var result = await _testClient.GetJobsAsync(expectedUri);

        // Assert
        Assert.Equal(expectedResponse.Content, result);
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri == expectedUri),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetJobsAsync_PostRequest_Success_ReturnsContent()
    {
        // Arrange
        var expectedUri = new Uri("https://example.com/api/jobs");
        var requestContent = new StringContent("request data");
        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("test content")
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == expectedUri &&
                    req.Content == requestContent),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        // Act
        var result = await _testClient.GetJobsAsync(expectedUri, usePost: true, requestContent: requestContent);

        // Assert
        Assert.Equal(expectedResponse.Content, result);
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri == expectedUri &&
                req.Content == requestContent),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetJobsAsync_NonSuccessStatusCode_ThrowsException()
    {
        // Arrange
        var expectedUri = new Uri("https://example.com/api/jobs");
        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent("Not found")
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _testClient.GetJobsAsync(expectedUri));

        Assert.Contains("NotFound", exception.Message);
    }

    [Fact]
    public async Task GetJobsAsync_HttpRequestException_ThrowsException()
    {
        // Arrange
        var expectedUri = new Uri("https://example.com/api/jobs");

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _testClient.GetJobsAsync(expectedUri));
    }
}
