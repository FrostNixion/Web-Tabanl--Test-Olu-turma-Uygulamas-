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

    public TeacherController(IRepository repository, UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
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
}
