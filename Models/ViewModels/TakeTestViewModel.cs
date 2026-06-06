using TestCreationSystem.Models;

namespace TestCreationSystem.Models.ViewModels;

public class TakeTestViewModel
{
    public int TestId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public string TestDescription { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
    public List<TestQuestionViewModel> Questions { get; set; } = new List<TestQuestionViewModel>();
}

public class TestQuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Points { get; set; }
    public List<TestOptionViewModel> Options { get; set; } = new List<TestOptionViewModel>();
}

public class TestOptionViewModel
{
    public int OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}
