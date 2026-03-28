using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dtos;

public class DescriptionCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Job ad ID must be greater than zero.")]
    public int Id { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public string? Workstyle { get; set; }
    public string? AboutProject { get; set; }

    public DescriptionCreateDto(
        int id,
        string? description,
        string? requirements,
        string? benefits,
        string? workstyle,
        string? aboutProject)
    {
        Id = id;
        Description = description;
        Requirements = requirements;
        Benefits = benefits;
        Workstyle = workstyle;
        AboutProject = aboutProject;
    }
}
