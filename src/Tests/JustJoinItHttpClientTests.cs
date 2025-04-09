using System.Net;
using System.Net.Http.Json;
using Application.Dtos.JustJoinIt;
using Domain.Enums;
using Infrastructure.Services.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Tests;
public class JustJoinItHttpClientTests
{
    private const string BaseUrl = "https://api.justjoin.it/";

    private static IConfiguration GetConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JustJoinIt:Url", BaseUrl }
            })
            .Build();

    private static HttpClient CreateHttpClient(HttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    private static HttpResponseMessage CreateHttpResponse(object content)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(content)
        };
    }

    private JustJoinItResponse CreateResponse(int page = 1, int totalPages = 1, int jobCount = 1)
    {
        return new JustJoinItResponse
        {
            Jobs = Enumerable.Range(1, jobCount).Select(i => new JustJoinItResponse.JustJoinItJob
            {
                Slug = $"slug-{page}-{i}",
                Title = $"Job {i}",
                WorkplaceType = "Remote",
                WorkingTime = "Full-time",
                ExperienceLevel = "Mid",
                EmploymentTypes = new()
                {
                    new JustJoinItResponse.JustJoinItEmploymentType
                    {
                        Type = "B2B",
                        FromPln = 10000,
                        ToPln = 20000,
                        Currency = "PLN",
                        Unit = "month"
                    }
                },
                CategoryId = JustJoinItCategory.Game,
                Multilocation = new()
                {
                    new JustJoinItResponse.JustJoinItMultilocation
                    {
                        City = "Warsaw",
                        Slug = "warsaw",
                        Street = "Main St",
                        Latitude = 52.2297,
                        Longitude = 21.0122
                    }
                },
                City = "Warsaw",
                Street = "Main St",
                Latitude = 52.2297,
                Longitude = 21.0122,
                RemoteInterview = true,
                CompanyName = "TestCompany",
                CompanyLogoThumbUrl = "https://logo.url",
                PublishedAt = DateTime.UtcNow,
                OpenToHireUkrainians = true
            }).ToList(),
            MetaData = new JustJoinItResponse.JustJoinItMetadata
            {
                Page = page,
                TotalPages = totalPages,
                PrevPage = page > 1 ? page - 1 : null,
                NextPage = page < totalPages ? page + 1 : null,
                TotalItems = jobCount
            }
        };
    }

    [Fact]
    public async Task GetJobsAsync_ReturnsJobs_WhenSinglePage()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var response = CreateResponse();

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => CreateHttpResponse(response));

        var httpClient = CreateHttpClient(mockHandler.Object);
        var config = GetConfiguration();
        var logger = new Mock<ILogger<JustJoinItHttpClient>>().Object;

        var sut = new JustJoinItHttpClient(httpClient, config, logger);

        // Act
        var jobs = await sut.GetJobsAsync();

        // Assert
        Assert.NotNull(jobs);
        Assert.Single(jobs);
    }

    [Fact]
    public async Task GetJobsAsync_ReturnsJobs_FromMultiplePages()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        var page1 = CreateResponse(1, 3, 1);
        var page2 = CreateResponse(2, 3, 2);
        var page3 = CreateResponse(3, 3, 3);

        var responses = new Queue<HttpResponseMessage>(new[]
        {
            CreateHttpResponse(page1),
            CreateHttpResponse(page2),
            CreateHttpResponse(page3)
        });

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => responses.Dequeue());

        var httpClient = CreateHttpClient(mockHandler.Object);
        var config = GetConfiguration();
        var logger = new Mock<ILogger<JustJoinItHttpClient>>().Object;

        var sut = new JustJoinItHttpClient(httpClient, config, logger);

        // Act
        var jobs = await sut.GetJobsAsync();

        // Assert
        Assert.NotNull(jobs);
        Assert.Equal(6, jobs.Count());
    }

    [Fact]
    public async Task GetJobsAsync_ThrowsException_WhenResponseIsNull()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        var emptyResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create<object?>(null)
        };

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(emptyResponse);

        var httpClient = CreateHttpClient(mockHandler.Object);
        var config = GetConfiguration();
        var logger = new Mock<ILogger<JustJoinItHttpClient>>().Object;

        var sut = new JustJoinItHttpClient(httpClient, config, logger);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => sut.GetJobsAsync());
    }

    [Fact]
    public async Task GetJobsAsync_ReturnsEmptyList_WhenJobsEmpty()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var response = CreateResponse(jobCount: 0);

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(CreateHttpResponse(response));

        var httpClient = CreateHttpClient(mockHandler.Object);
        var config = GetConfiguration();
        var logger = new Mock<ILogger<JustJoinItHttpClient>>().Object;

        var sut = new JustJoinItHttpClient(httpClient, config, logger);

        // Act
        var jobs = await sut.GetJobsAsync();

        // Assert
        Assert.NotNull(jobs);
        Assert.Empty(jobs);
    }
}
