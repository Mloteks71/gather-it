namespace JobReadApi.Application.Entities;

public class JobAdSkill
{
    public int JobAdId { get; set; }
    public int SkillId { get; set; }

    public JobAd JobAd { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

