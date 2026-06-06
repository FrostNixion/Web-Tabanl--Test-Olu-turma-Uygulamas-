using System.ComponentModel.DataAnnotations;

namespace TestCreationSystem.Models.ViewModels;

public class CreateTestViewModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Range(1, 180)]
    public int DurationInMinutes { get; set; }

    public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
}

public class QuestionViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    [Range(1, 100)]
    public int Points { get; set; } = 1;

    public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
}

public class OptionViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
