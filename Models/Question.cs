using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestCreationSystem.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    public int TestId { get; set; }

    [ForeignKey(nameof(TestId))]
    public Test? Test { get; set; }

    [Required]
    [StringLength(1000)]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public int Points { get; set; } = 1;

    public ICollection<Option> Options { get; set; } = new List<Option>();
}
