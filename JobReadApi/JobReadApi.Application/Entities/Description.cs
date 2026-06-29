namespace JobReadApi.Application.Entities;

public class Description
{
    public int DescriptionId { get; set; }
    public int JobAdId { get; set; }
    public string? DescriptionText { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public string? Workstyle { get; set; }
    public string? AboutProject { get; set; }

    public JobAd JobAd { get; set; } = null!;
}

