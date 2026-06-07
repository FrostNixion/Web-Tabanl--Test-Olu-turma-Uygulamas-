using TestCreationSystem.Models;

namespace TestCreationSystem.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTests { get; set; }
    public List<ApplicationUser> RecentUsers { get; set; } = new();
    public List<Test> AllTests { get; set; } = new();
    public List<StudentSuccessRateViewModel> StudentSuccessRates { get; set; } = new();
}

public class StudentSuccessRateViewModel
{
    public ApplicationUser Student { get; set; } = null!;
    public int TotalTestsTaken { get; set; }
    public double AverageScore { get; set; }
}

public class UserWithRoleViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}

public class CreateUserViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
}
