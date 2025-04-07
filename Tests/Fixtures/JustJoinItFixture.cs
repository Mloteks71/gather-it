using Application.Dtos.JustJoinIt;
using Domain.Enums;
using static Application.Dtos.JustJoinIt.JustJoinItResponse;

namespace Tests.Fixtures;
public class JustJoinItFixture
{
    public JustJoinItResponse CreateValidResponse(int page = 1, int totalPages = 1, List<JustJoinItJob>? jobs = null)
    {
        return new JustJoinItResponse
        {
            Jobs = jobs ?? new List<JustJoinItJob> { CreateValidJob() },
            MetaData = CreateMetaData(page, totalPages)
        };
    }

    public JustJoinItMetadata CreateMetaData(int page, int totalPages)
    {
        return new JustJoinItMetadata
        {
            NextPage = page < totalPages ? page + 1 : null,
            Page = page,
            PrevPage = page > 1 ? page - 1 : null,
            TotalItems = 1,
            TotalPages = totalPages
        };
    }

    public JustJoinItJob CreateValidJob(
        string slug = "test-job",
        string title = "Test Job",
        string workplaceType = "Remote",
        string companyName = "Test Company")
    {
        return new JustJoinItJob
        {
            Slug = slug,
            Title = title,
            WorkplaceType = workplaceType,
            EmploymentTypes = new List<JustJoinItEmploymentType>
                {
                    new JustJoinItEmploymentType
                    {
                        Type = "b2b",
                        FromPln = 10000,
                        ToPln = 20000,
                        Currency = "pln",
                        Unit = "monthly",
                        Gross = true,
                        // Initialize all required properties
                        FromChf = null,
                        FromEur = null,
                        FromGbp = null,
                        FromUsd = null,
                        ToChf = null,
                        ToEur = null,
                        ToGbp = null,
                        ToUsd = null
                    }
                },
            RequiredSkills = new List<string> { "C#", ".NET" },
            CompanyName = companyName,
            CompanyLogoThumbUrl = "https://example.com/logo.png",
            PublishedAt = DateTime.UtcNow,
            Multilocation = new List<JustJoinItMultilocation>
                {
                    new JustJoinItMultilocation
                    {
                        City = "Warsaw",
                        Slug = slug,
                        Street = "Test Street",
                        Latitude = 52.23,
                        Longitude = 21.01
                    }
                },
            // Initialize all required properties
            City = "Warsaw",
            Street = "Test Street",
            Latitude = 52.23,
            Longitude = 21.01,
            RemoteInterview = true,
            OpenToHireUkrainians = true,
            ExperienceLevel = "senior",
            WorkingTime = "full_time",
            CategoryId = JustJoinItCategory.DevOps,
            NiceToHaveSkills = new List<string>(),
            Languages = new List<JustJoinItLanguage>()
        };
    }

    public JustJoinItResponse CreateEmptyResponse()
    {
        return new JustJoinItResponse
        {
            Jobs = new List<JustJoinItJob>(),
            MetaData = new JustJoinItMetadata
            {
                NextPage = null,
                Page = 1,
                PrevPage = null,
                TotalItems = 0,
                TotalPages = 1
            }
        };
    }
}
