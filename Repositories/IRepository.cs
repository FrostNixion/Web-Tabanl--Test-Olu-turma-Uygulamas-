using TestCreationSystem.Models;

namespace TestCreationSystem.Repositories;

public interface IRepository
{
    // User operations
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);

    // Test operations
    Task<Test?> GetTestByIdAsync(int id);
    Task<List<Test>> GetAllTestsAsync();
    Task<List<Test>> GetTestsByTeacherIdAsync(string teacherId);
    Task AddTestAsync(Test test);
    Task UpdateTestAsync(Test test);
    Task DeleteTestAsync(int id);

    // Question operations
    Task<Question?> GetQuestionByIdAsync(int id);
    Task<List<Question>> GetQuestionsByTestIdAsync(int testId);
    Task AddQuestionAsync(Question question);
    Task UpdateQuestionAsync(Question question);
    Task DeleteQuestionAsync(int id);

    // Option operations
    Task<Option?> GetOptionByIdAsync(int id);
    Task<List<Option>> GetOptionsByQuestionIdAsync(int questionId);
    Task AddOptionAsync(Option option);
    Task UpdateOptionAsync(Option option);
    Task DeleteOptionAsync(int id);

    // TestResult operations
    Task<TestResult?> GetTestResultByIdAsync(int id);
    Task<List<TestResult>> GetTestResultsByTestIdAsync(int testId);
    Task<List<TestResult>> GetTestResultsByStudentIdAsync(string studentId);
    Task<TestResult?> GetTestResultByStudentAndTestAsync(string studentId, int testId);
    Task AddTestResultAsync(TestResult testResult);
    Task UpdateTestResultAsync(TestResult testResult);

    // Save changes
    Task SaveChangesAsync();
}
