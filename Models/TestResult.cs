using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCreationSystem.Models;

public class TestResult
{
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [ForeignKey(nameof(StudentId))]
    public ApplicationUser? Student { get; set; }

    [Required]
    public int TestId { get; set; }

    [ForeignKey(nameof(TestId))]
    public Test? Test { get; set; }

    [Required]
    public int Score { get; set; }

    public DateTime TakenAt { get; set; } = DateTime.UtcNow;
}
