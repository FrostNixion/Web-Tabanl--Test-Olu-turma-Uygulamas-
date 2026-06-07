using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestCreationSystem.Data;
using TestCreationSystem.Models;

namespace TestCreationSystem.Repositories;

public class Repository : IRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public Repository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // User operations
    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Name)
            .ThenBy(u => u.Surname)
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string role)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(role);
        return usersInRole
            .OrderBy(u => u.Name)
            .ThenBy(u => u.Surname)
            .ToList();
    }

    public async Task AddUserAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    }

    // Test operations
    public async Task<Test?> GetTestByIdAsync(int id)
    {
        return await _context.Tests
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .Include(t => t.CreatedByTeacher)
            .Include(t => t.TestResults)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Test>> GetAllTestsAsync()
    {
        return await _context.Tests
            .Include(t => t.Questions)
            .Include(t => t.CreatedByTeacher)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Test>> GetTestsByTeacherIdAsync(string teacherId)
    {
        return await _context.Tests
            .Include(t => t.Questions)
            .Include(t => t.TestResults)
            .ThenInclude(tr => tr.Student)
            .Where(t => t.CreatedByTeacherId == teacherId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task AddTestAsync(Test test)
    {
        _context.Tests.Add(test);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTestAsync(Test test)
    {
        _context.Tests.Update(test);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTestAsync(int id)
    {
        var test = await _context.Tests.FindAsync(id);
        if (test != null)
        {
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
        }
    }

    // Question operations
    public async Task<Question?> GetQuestionByIdAsync(int id)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<List<Question>> GetQuestionsByTestIdAsync(int testId)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .Where(q => q.TestId == testId)
            .ToListAsync();
    }

    public async Task AddQuestionAsync(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateQuestionAsync(Question question)
    {
        _context.Questions.Update(question);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteQuestionAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question != null)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }

    // Option operations
    public async Task<Option?> GetOptionByIdAsync(int id)
    {
        return await _context.Options.FindAsync(id);
    }

    public async Task<List<Option>> GetOptionsByQuestionIdAsync(int questionId)
    {
        return await _context.Options
            .Where(o => o.QuestionId == questionId)
            .ToListAsync();
    }

    public async Task AddOptionAsync(Option option)
    {
        _context.Options.Add(option);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOptionAsync(Option option)
    {
        _context.Options.Update(option);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOptionAsync(int id)
    {
        var option = await _context.Options.FindAsync(id);
        if (option != null)
        {
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
        }
    }

    // TestResult operations
    public async Task<TestResult?> GetTestResultByIdAsync(int id)
    {
        return await _context.TestResults
            .Include(tr => tr.Student)
            .Include(tr => tr.Test)
            .FirstOrDefaultAsync(tr => tr.Id == id);
    }

    public async Task<List<TestResult>> GetTestResultsByTestIdAsync(int testId)
    {
        return await _context.TestResults
            .Include(tr => tr.Student)
            .Include(tr => tr.Test)
            .Where(tr => tr.TestId == testId)
            .OrderByDescending(tr => tr.TakenAt)
            .ToListAsync();
    }

    public async Task<List<TestResult>> GetTestResultsByStudentIdAsync(string studentId)
    {
        return await _context.TestResults
            .Include(tr => tr.Test)
            .ThenInclude(t => t.Questions)
            .ThenInclude(q => q.Options)
            .Include(tr => tr.Student)
            .Where(tr => tr.StudentId == studentId)
            .OrderByDescending(tr => tr.TakenAt)
            .ToListAsync();
    }

    public async Task<TestResult?> GetTestResultByStudentAndTestAsync(string studentId, int testId)
    {
        return await _context.TestResults
            .FirstOrDefaultAsync(tr => tr.StudentId == studentId && tr.TestId == testId);
    }

    public async Task<List<TestResult>> GetAllTestResultsAsync()
    {
        return await _context.TestResults
            .Include(tr => tr.Test)
            .Include(tr => tr.Student)
            .ToListAsync();
    }

    public async Task AddTestResultAsync(TestResult testResult)
    {
        _context.TestResults.Add(testResult);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTestResultAsync(TestResult testResult)
    {
        _context.TestResults.Update(testResult);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTestResultAsync(int id)
    {
        var testResult = await _context.TestResults.FindAsync(id);
        if (testResult != null)
        {
            _context.TestResults.Remove(testResult);
            await _context.SaveChangesAsync();
        }
    }

    // Save changes
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
