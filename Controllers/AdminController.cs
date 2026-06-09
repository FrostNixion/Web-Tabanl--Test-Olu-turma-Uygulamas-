using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestCreationSystem.Models;
using TestCreationSystem.Models.ViewModels;
using TestCreationSystem.Repositories;

namespace TestCreationSystem.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(IRepository repository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _repository = repository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var allUsers = await _repository.GetAllUsersAsync();
        var teachers = await _repository.GetUsersByRoleAsync("Teacher");
        var students = await _repository.GetUsersByRoleAsync("Student");
        var allTests = await _repository.GetAllTestsAsync();
        var allTestResults = await _repository.GetAllTestResultsAsync();

        // Calculate student success rates
        var studentSuccessRates = new List<StudentSuccessRateViewModel>();
        foreach (var student in students)
        {
            var studentResults = allTestResults.Where(tr => tr.StudentId == student.Id).ToList();
            if (studentResults.Any())
            {
                var totalScore = studentResults.Sum(tr => tr.Score);
                var totalPossible = 0;
                foreach (var result in studentResults)
                {
                    var test = allTests.FirstOrDefault(t => t.Id == result.TestId);
                    if (test != null)
                    {
                        totalPossible += test.Questions.Sum(q => q.Points);
                    }
                }
                var successRate = totalPossible > 0 ? (double)totalScore / totalPossible * 100 : 0;
                studentSuccessRates.Add(new StudentSuccessRateViewModel
                {
                    Student = student,
                    TotalTestsTaken = studentResults.Count,
                    AverageScore = successRate
                });
            }
        }

        var model = new AdminDashboardViewModel
        {
            TotalUsers = allUsers.Count,
            TotalTeachers = teachers.Count,
            TotalStudents = students.Count,
            TotalTests = allTests.Count,
            RecentUsers = allUsers.OrderByDescending(u => u.Id).Take(5).ToList(),
            AllTests = allTests.OrderByDescending(t => t.CreatedAt).ToList(),
            StudentSuccessRates = studentSuccessRates.OrderByDescending(s => s.AverageScore).ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Users()
    {
        var users = await _repository.GetAllUsersAsync();
        var usersWithRoles = new List<UserWithRoleViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            usersWithRoles.Add(new UserWithRoleViewModel
            {
                User = user,
                Roles = roles.ToList()
            });
        }

        return View(usersWithRoles);
    }

    public IActionResult CreateUser()
    {
        var model = new CreateUserViewModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Name = model.Name,
            Surname = model.Surname,
            Role = model.Role,
            EmailConfirmed = true
        };

        try
        {
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            await _repository.AddUserAsync(user, model.Password);
            var addToRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!addToRoleResult.Succeeded)
            {
                foreach (var error in addToRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            return RedirectToAction("Users");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> TestResults(int id)
    {
        var test = await _repository.GetTestByIdAsync(id);
        if (test == null)
        {
            return NotFound();
        }

        var results = await _repository.GetTestResultsByTestIdAsync(id);
        ViewBag.TestTitle = test.Title;
        return View("~/Views/Teacher/TestResults.cshtml", results);
    }

    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Id == id)
        {
            ModelState.AddModelError(string.Empty, "Kendi hesabınızı silemezsiniz.");
            return RedirectToAction("Users");
        }

        await _repository.DeleteUserAsync(id);
        return RedirectToAction("Users");
    }
}
