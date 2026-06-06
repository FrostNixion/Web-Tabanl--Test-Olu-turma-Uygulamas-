using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCreationSystem.Models;

public class Test
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public int DurationInMinutes { get; set; }

    [Required]
    public string CreatedByTeacherId { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedByTeacherId))]
    public ApplicationUser? CreatedByTeacher { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
