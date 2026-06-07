using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TestCreationSystem.Models;
using TestCreationSystem.Models.ViewModels;
using TestCreationSystem.Repositories;

namespace TestCreationSystem.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherController : Controller
{
    private readonly IRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public TeacherController(IRepository repository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _repository = repository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var tests = await _repository.GetTestsByTeacherIdAsync(user.Id);
        return View(tests);
    }

    public IActionResult CreateTest()
    {
        var model = new CreateTestViewModel
        {
            Questions = new List<QuestionViewModel>
            {
                new QuestionViewModel
                {
                    Options = new List<OptionViewModel>
                    {
                        new OptionViewModel(),
                        new OptionViewModel(),
                        new OptionViewModel(),
                        new OptionViewModel()
                    }
                }
            }
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTest(CreateTestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var test = new Test
        {
            Title = model.Title,
            Description = model.Description,
            DurationInMinutes = model.DurationInMinutes,
            CreatedByTeacherId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddTestAsync(test);

        foreach (var questionVm in model.Questions)
        {
            var question = new Question
            {
                TestId = test.Id,
                QuestionText = questionVm.QuestionText,
                Points = questionVm.Points
            };

            await _repository.AddQuestionAsync(question);

            foreach (var optionVm in questionVm.Options)
            {
                var option = new Option
                {
                    QuestionId = question.Id,
                    OptionText = optionVm.OptionText,
                    IsCorrect = optionVm.IsCorrect
                };

                await _repository.AddOptionAsync(option);
            }
        }

        return RedirectToAction("Dashboard");
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
        return View(results);
    }

    public async Task<IActionResult> DeleteTest(int id)
    {
        var test = await _repository.GetTestByIdAsync(id);
        if (test == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || test.CreatedByTeacherId != user.Id)
        {
            return Forbid();
        }

        await _repository.DeleteTestAsync(id);
        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> DeleteTestResult(int id)
    {
        var result = await _repository.GetTestResultByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }

        var test = await _repository.GetTestByIdAsync(result.TestId);
        if (test == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || test.CreatedByTeacherId != user.Id)
        {
            return Forbid();
        }

        await _repository.DeleteTestResultAsync(id);
        return RedirectToAction("TestResults", new { id = result.TestId });
    }

    public async Task<IActionResult> Students()
    {
        var students = await _repository.GetUsersByRoleAsync("Student");
        return View(students);
    }

    public IActionResult CreateStudent()
    {
        var model = new CreateUserViewModel { Role = "Student" };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStudent(CreateUserViewModel model)
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
            Role = "Student",
            EmailConfirmed = true
        };

        try
        {
            await _repository.AddUserAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, "Student");
            return RedirectToAction("Students");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> DeleteStudent(string id)
    {
        var student = await _repository.GetUserByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var isInStudentRole = await _userManager.IsInRoleAsync(student, "Student");
        if (!isInStudentRole)
        {
            return Forbid();
        }

        await _repository.DeleteUserAsync(id);
        return RedirectToAction("Students");
    }
}
