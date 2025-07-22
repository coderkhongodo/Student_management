using Microsoft.AspNetCore.Identity;
using StudentManagementSystem.Constants;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create roles
            await CreateRolesAsync(roleManager);

            // Create default users
            await CreateDefaultUsersAsync(userManager);
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task CreateDefaultUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Create Admin user
            if (await userManager.FindByEmailAsync("admin@sms.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@sms.com",
                    Email = "admin@sms.com",
                    FirstName = "System",
                    LastName = "Administrator",
                    EmployeeId = "EMP001",
                    Salary = 50000000,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
                }
            }

            // Create Teacher user
            if (await userManager.FindByEmailAsync("teacher@sms.com") == null)
            {
                var teacher = new ApplicationUser
                {
                    UserName = "teacher@sms.com",
                    Email = "teacher@sms.com",
                    FirstName = "Nguyễn",
                    LastName = "Văn Giáo",
                    EmployeeId = "TCH001",
                    Salary = 30000000,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(teacher, "Teacher123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacher, Roles.Teacher);
                }
            }

            // Create Student user
            if (await userManager.FindByEmailAsync("student@sms.com") == null)
            {
                var student = new ApplicationUser
                {
                    UserName = "student@sms.com",
                    Email = "student@sms.com",
                    FirstName = "Trần",
                    LastName = "Thị Học",
                    StudentId = "STU001",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(student, "Student123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, Roles.Student);
                }
            }
        }
    }
}
