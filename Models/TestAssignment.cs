using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCreationSystem.Models;

public class TestAssignment
{
    [Required]
    public int TestId { get; set; }

    [ForeignKey(nameof(TestId))]
    public Test? Test { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [ForeignKey(nameof(StudentId))]
    public ApplicationUser? Student { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
