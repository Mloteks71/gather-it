namespace Domain.Entities;
public record Description
{
    public Description() { }

    public Description(int jobAdId, string? description, string? requirements, string? benefits, string? workstyle, string? aboutProject)
    {
        JobAdId = jobAdId;
        DescriptionText = description;
        Requirements = requirements;
        Benefits = benefits;
        Workstyle = workstyle;
        AboutProject = aboutProject;
    }

    public string? DescriptionText { get; set; }
    public string DescriptionBlock { get => DescriptionText ?? $"{Requirements}\n{Benefits}\n{Workstyle}\n{AboutProject}"; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public string? Workstyle { get; set; }
    public string? AboutProject { get; set; }
    public int JobAdId { get; set; }
    public JobAd JobAd { get; set; } = null!;
}
