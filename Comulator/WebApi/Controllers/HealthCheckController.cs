using Application.Interfaces.MessageSenders;
using Application.Models.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : BaseController
{
    private readonly IMappingServiceMessageSender _mappingServiceMessageSender;

    public HealthCheckController(
        ILogger<HealthCheckController> logger,
        IMappingServiceMessageSender mappingServiceMessageSender) : base(logger)
    {
        _mappingServiceMessageSender = mappingServiceMessageSender;
    }

    [HttpGet]
    public ActionResult Get()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    [HttpPost("mock")]
    public async Task<ActionResult> SendMockData()
    {
        List<CommonJobAdDto> mockJobAds =
        [
            new CommonJobAdDto
            {
                Id = "abc-123",
                Slug = "senior-rust-developer-acme",
                Title = "Senior Rust Developer",
                CompanyName = "Acme Corp",
                SourceSite = Site.JustJoinIt,
                Skills = ["Rust", "PostgreSQL", "RabbitMQ"],
                WorkplaceTypes = ["Remote"],
                ExperienceLevels = ["Senior"],
                Locations = ["Warsaw", "Krakow"],
                Salaries =
                [
                    new SalaryRangeDto { From = 25000m, To = 35000m, Currency = "PLN", ContractType = "B2B" },
                    new SalaryRangeDto { From = 20000m, To = 28000m, Currency = "PLN", ContractType = "UoP" }
                ],
                PublishedAt = DateTime.UtcNow,
                LogoUrl = "https://example.com/logo1.png"
            },
            new CommonJobAdDto
            {
                Id = "def-456",
                Slug = "mid-dotnet-developer-globex",
                Title = "Mid .NET Developer",
                CompanyName = "Globex Corporation",
                SourceSite = Site.TheProtocolIt,
                Skills = ["C#", ".NET", "Azure"],
                WorkplaceTypes = ["Hybrid"],
                ExperienceLevels = ["Mid"],
                Locations = ["Wroclaw"],
                Salaries =
                [
                    new SalaryRangeDto { From = 15000m, To = 22000m, Currency = "PLN", ContractType = "B2B" }
                ],
                PublishedAt = DateTime.UtcNow
            },
            new CommonJobAdDto
            {
                Id = "ghi-789",
                Slug = "junior-go-developer-initech",
                Title = "Junior Go Developer",
                CompanyName = "Initech",
                SourceSite = Site.JustJoinIt,
                Skills = ["Go", "Docker"],
                WorkplaceTypes = ["OnSite"],
                ExperienceLevels = ["Junior"],
                Locations = ["Gdansk"]
            }
        ];

        await _mappingServiceMessageSender.SendMappedJobAdsAsync(mockJobAds);

        Logger.LogInformation("Sent {Count} mock job ads to mapping queue", mockJobAds.Count);

        return Ok(new { status = "sent", count = mockJobAds.Count });
    }
}
