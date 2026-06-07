using TestCreationSystem.Models;

namespace TestCreationSystem.Models.ViewModels;

public class TestResultViewModel
{
    public int TestId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalPoints { get; set; }
    public DateTime TakenAt { get; set; }
    public List<ResultQuestionViewModel> Questions { get; set; } = new List<ResultQuestionViewModel>();
}

public class ResultQuestionViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Points { get; set; }
    public List<ResultOptionViewModel> Options { get; set; } = new List<ResultOptionViewModel>();
}

public class ResultOptionViewModel
{
    public int OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
