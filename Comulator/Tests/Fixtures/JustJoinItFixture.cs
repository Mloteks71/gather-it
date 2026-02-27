using Application.Models.Responses;

namespace Tests.Fixtures;

public class JustJoinItFixture
{
    public JustJoinItResponse CreateValidResponse(int page = 1, int totalPages = 1, List<JobAd>? jobs = null)
    {
        return new JustJoinItResponse
        {
            Data = jobs ?? new List<JobAd> { CreateValidJob() },
            Meta = CreateMetaData(page, totalPages)
        };
    }

    public MetaData CreateMetaData(int page, int totalPages)
    {
        return new MetaData
        {
            NextPage = page < totalPages ? page + 1 : 0,
            Page = page,
            PrevPage = page > 1 ? page - 1 : null,
            TotalItems = 1,
            TotalPages = totalPages
        };
    }

    public JobAd CreateValidJob(
        string slug = "test-job",
        string title = "Test Job",
        string workplaceType = "Remote",
        string companyName = "Test Company")
    {
        return new JobAd
        {
            Slug = slug,
            Title = title,
            WorkplaceType = workplaceType,
            EmploymentTypes = new List<EmploymentType>
            {
                new EmploymentType
                {
                    Type = "b2b",
                    FromPln = 10000,
                    ToPln = 20000,
                    Currency = "pln",
                    Unit = "monthly",
                    Gross = true,
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
            Multilocation = new List<Location>
            {
                new Location
                {
                    City = "Warsaw",
                    Slug = slug,
                    Street = "Test Street",
                    Latitude = 52.23,
                    Longitude = 21.01
                }
            },
            City = "Warsaw",
            Street = "Test Street",
            Latitude = "52.23",
            Longitude = "21.01",
            RemoteInterview = true,
            OpenToHireUkrainians = true,
            ExperienceLevel = "senior",
            WorkingTime = "full_time",
            CategoryId = 12,
            NiceToHaveSkills = new List<string>(),
            Languages = new List<Language>()
        };
    }

    public JustJoinItResponse CreateEmptyResponse()
    {
        return new JustJoinItResponse
        {
            Data = new List<JobAd>(),
            Meta = new MetaData
            {
                NextPage = 0,
                Page = 1,
                PrevPage = null,
                TotalItems = 0,
                TotalPages = 1
            }
        };
    }
}
