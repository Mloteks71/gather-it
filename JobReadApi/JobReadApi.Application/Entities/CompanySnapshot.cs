namespace JobReadApi.Application.Entities;

public class CompanySnapshot
{
    public int CompanySnapshotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int[] JobAdIds { get; set; } = [];
}

