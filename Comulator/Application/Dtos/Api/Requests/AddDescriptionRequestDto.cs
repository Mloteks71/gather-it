using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Api.Requests;

public record AddDescriptionRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Job ad ID must be greater than zero.")]
    public int Id { get; set; }

    [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters.")]
    public string? Description { get; set; }

    [StringLength(5000, ErrorMessage = "Requirements cannot exceed 5000 characters.")]
    public string? Requirements { get; set; }

    [StringLength(5000, ErrorMessage = "Benefits cannot exceed 5000 characters.")]
    public string? Benefits { get; set; }

    [StringLength(500, ErrorMessage = "Workstyle cannot exceed 500 characters.")]
    public string? Workstyle { get; set; }

    [StringLength(5000, ErrorMessage = "AboutProject cannot exceed 5000 characters.")]
    public string? AboutProject { get; set; }
}
