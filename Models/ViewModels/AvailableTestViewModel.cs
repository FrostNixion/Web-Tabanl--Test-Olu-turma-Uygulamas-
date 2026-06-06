using TestCreationSystem.Models;

namespace TestCreationSystem.Models.ViewModels;

public class AvailableTestViewModel
{
    public Test Test { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public int? Score { get; set; }
}
