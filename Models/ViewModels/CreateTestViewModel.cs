using System.ComponentModel.DataAnnotations;

namespace TestCreationSystem.Models.ViewModels;

public class CreateTestViewModel
{
    [Required(ErrorMessage = "Sınav başlığı zorunludur")]
    [StringLength(200)]
    [Display(Name = "Sınav Başlığı")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Süre zorunludur")]
    [Range(1, 180, ErrorMessage = "Süre 1-180 dakika arasında olmalıdır")]
    [Display(Name = "Süre (Dakika)")]
    public int DurationInMinutes { get; set; }

    public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
}

public class QuestionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Soru metni zorunludur")]
    [StringLength(1000)]
    [Display(Name = "Soru Metni")]
    public string QuestionText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Puan zorunludur")]
    [Range(1, 100, ErrorMessage = "Puan 1-100 arasında olmalıdır")]
    [Display(Name = "Puan")]
    public int Points { get; set; } = 1;

    public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
}

public class OptionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Seçenek metni zorunludur")]
    [StringLength(500)]
    [Display(Name = "Seçenek Metni")]
    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
