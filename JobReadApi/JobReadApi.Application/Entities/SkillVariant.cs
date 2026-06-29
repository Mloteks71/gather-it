namespace JobReadApi.Application.Entities;

public class SkillVariant
{
    public int SkillVariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SkillId { get; set; }

    public Skill Skill { get; set; } = null!;
}

