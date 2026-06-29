namespace JobReadApi.Application.Entities;

public class SkillSnapshot
{
    public int SkillSnapshotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int[] JobAdIds { get; set; } = [];
}

