namespace JobReadApi.Application.Entities;

public class Skill
{
    public int SkillId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<SkillVariant> Variants { get; set; } = [];
    public ICollection<JobAdSkill> JobAdSkills { get; set; } = [];
}

