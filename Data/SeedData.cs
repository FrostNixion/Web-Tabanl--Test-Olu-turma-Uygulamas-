using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestCreationSystem.Models;

namespace TestCreationSystem.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure database is created
        context.Database.EnsureCreated();

        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Teacher"))
        {
            await roleManager.CreateAsync(new IdentityRole("Teacher"));
        }

        if (!await roleManager.RoleExistsAsync("Student"))
        {
            await roleManager.CreateAsync(new IdentityRole("Student"));
        }

        // Seed Teacher User
        var teacherUser = await userManager.FindByEmailAsync("teacher@test.com");
        if (teacherUser == null)
        {
            teacherUser = new ApplicationUser
            {
                UserName = "teacher@test.com",
                Email = "teacher@test.com",
                Name = "Teacher",
                Surname = "User",
                Role = "Teacher",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(teacherUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(teacherUser, "Teacher");
            }
        }

        // Seed Student User
        var studentUser = await userManager.FindByEmailAsync("student@test.com");
        if (studentUser == null)
        {
            studentUser = new ApplicationUser
            {
                UserName = "student@test.com",
                Email = "student@test.com",
                Name = "Student",
                Surname = "User",
                Role = "Student",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(studentUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }

        // Seed Sample Test
        if (!context.Tests.Any())
        {
            var test = new Test
            {
                Title = "General Knowledge Quiz",
                Description = "A sample quiz to test your general knowledge",
                DurationInMinutes = 10,
                CreatedByTeacherId = teacherUser!.Id,
                CreatedAt = DateTime.UtcNow
            };

            var question1 = new Question
            {
                TestId = test.Id,
                QuestionText = "What is the capital of France?",
                Points = 1
            };

            question1.Options.Add(new Option { QuestionId = question1.Id, OptionText = "London", IsCorrect = false });
            question1.Options.Add(new Option { QuestionId = question1.Id, OptionText = "Berlin", IsCorrect = false });
            question1.Options.Add(new Option { QuestionId = question1.Id, OptionText = "Paris", IsCorrect = true });
            question1.Options.Add(new Option { QuestionId = question1.Id, OptionText = "Madrid", IsCorrect = false });

            var question2 = new Question
            {
                TestId = test.Id,
                QuestionText = "Which planet is known as the Red Planet?",
                Points = 1
            };

            question2.Options.Add(new Option { QuestionId = question2.Id, OptionText = "Venus", IsCorrect = false });
            question2.Options.Add(new Option { QuestionId = question2.Id, OptionText = "Mars", IsCorrect = true });
            question2.Options.Add(new Option { QuestionId = question2.Id, OptionText = "Jupiter", IsCorrect = false });
            question2.Options.Add(new Option { QuestionId = question2.Id, OptionText = "Saturn", IsCorrect = false });

            var question3 = new Question
            {
                TestId = test.Id,
                QuestionText = "What is 2 + 2?",
                Points = 1
            };

            question3.Options.Add(new Option { QuestionId = question3.Id, OptionText = "3", IsCorrect = false });
            question3.Options.Add(new Option { QuestionId = question3.Id, OptionText = "4", IsCorrect = true });
            question3.Options.Add(new Option { QuestionId = question3.Id, OptionText = "5", IsCorrect = false });
            question3.Options.Add(new Option { QuestionId = question3.Id, OptionText = "6", IsCorrect = false });

            test.Questions.Add(question1);
            test.Questions.Add(question2);
            test.Questions.Add(question3);

            context.Tests.Add(test);
            await context.SaveChangesAsync();
        }
    }
}
