using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TestCreationSystem.Models;
using TestCreationSystem.Models.ViewModels;
using TestCreationSystem.Repositories;

namespace TestCreationSystem.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly IRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentController(IRepository repository, UserManager<ApplicationUser> userManager)
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

        var allTests = await _repository.GetAllTestsAsync();
        var studentResults = await _repository.GetTestResultsByStudentIdAsync(user.Id);
        var completedTestIds = studentResults.Select(tr => tr.TestId).ToHashSet();

        var availableTests = allTests.Select(t => new AvailableTestViewModel
        {
            Test = t,
            IsCompleted = completedTestIds.Contains(t.Id),
            Score = completedTestIds.Contains(t.Id) 
                ? studentResults.First(tr => tr.TestId == t.Id).Score 
                : (int?)null
        }).ToList();

        return View(availableTests);
    }

    public async Task<IActionResult> TakeTest(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var existingResult = await _repository.GetTestResultByStudentAndTestAsync(user.Id, id);
        if (existingResult != null)
        {
            return RedirectToAction("ViewResult", new { id = existingResult.Id });
        }

        var test = await _repository.GetTestByIdAsync(id);
        if (test == null)
        {
            return NotFound();
        }

        var model = new TakeTestViewModel
        {
            TestId = test.Id,
            TestTitle = test.Title,
            TestDescription = test.Description ?? "",
            DurationInMinutes = test.DurationInMinutes,
            Questions = test.Questions.Select(q => new TestQuestionViewModel
            {
                QuestionId = q.Id,
                QuestionText = q.QuestionText,
                Points = q.Points,
                Options = q.Options.Select(o => new TestOptionViewModel
                {
                    OptionId = o.Id,
                    OptionText = o.OptionText,
                    IsSelected = false
                }).ToList()
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitTest(TakeTestViewModel model, IFormCollection form)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var test = await _repository.GetTestByIdAsync(model.TestId);
        if (test == null)
        {
            return NotFound();
        }

        int totalScore = 0;
        int totalPoints = 0;

        for (int i = 0; i < model.Questions.Count; i++)
        {
            var questionVm = model.Questions[i];
            var question = test.Questions.FirstOrDefault(q => q.Id == questionVm.QuestionId);
            if (question != null)
            {
                totalPoints += question.Points;
                
                // Get the selected option index from the form
                string selectedOptionKey = $"Questions[{i}].SelectedOption";
                if (form.ContainsKey(selectedOptionKey))
                {
                    string? selectedValue = form[selectedOptionKey];
                    if (int.TryParse(selectedValue, out int selectedIndex) && selectedIndex >= 0 && selectedIndex < questionVm.Options.Count)
                    {
                        var selectedOption = questionVm.Options[selectedIndex];
                        var option = question.Options.FirstOrDefault(o => o.Id == selectedOption.OptionId);
                        if (option != null && option.IsCorrect)
                        {
                            totalScore += question.Points;
                        }
                    }
                }
            }
        }

        var testResult = new TestResult
        {
            StudentId = user.Id,
            TestId = test.Id,
            Score = totalScore,
            TakenAt = DateTime.UtcNow
        };

        await _repository.AddTestResultAsync(testResult);

        return RedirectToAction("ViewResult", new { id = testResult.Id });
    }

    public async Task<IActionResult> ViewResult(int id)
    {
        var result = await _repository.GetTestResultByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || result.StudentId != user.Id)
        {
            return Forbid();
        }

        var test = await _repository.GetTestByIdAsync(result.TestId);
        if (test == null)
        {
            return NotFound();
        }

        var totalPoints = test.Questions.Sum(q => q.Points);

        var model = new TestResultViewModel
        {
            TestId = test.Id,
            TestTitle = test.Title,
            Score = result.Score,
            TotalPoints = totalPoints,
            TakenAt = result.TakenAt,
            Questions = new List<ResultQuestionViewModel>()
        };

        return View(model);
    }
}
