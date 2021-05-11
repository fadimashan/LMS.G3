using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Bogus;
using LMS.Core.Entities;

namespace LMS.Data.Data
{
    public static class SeedData
    {
        private static Faker _fake;
        private static UserManager<ApplicationUser> _userManager;
        private static RoleManager<IdentityRole> _roleManager;

        public static async Task InitAsync(IServiceProvider services, string adminPW, string studentPW)
        {
            using (var context = new MvcDbContext(services.GetRequiredService<DbContextOptions<MvcDbContext>>()))
            {
                _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                _roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                _fake = new Faker("sv");

                var roleNames = new[] { "Teacher", "Student" };

                foreach (var roleName in roleNames)
                {
                    if (await _roleManager.RoleExistsAsync(roleName))
                    {
                        continue;
                    }

                    var role = new IdentityRole { Name = roleName };
                    var result = await _roleManager.CreateAsync(role);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
                }


                var TeacherEmail = "dimitris@lms.se";

                var foundTeacher = await _userManager.FindByEmailAsync(TeacherEmail);

                if (foundTeacher is not null) return;

                var admin = new ApplicationUser
                {
                    UserName = "dimitris",
                    Email = TeacherEmail,
                };

                var addAdminResult = await _userManager.CreateAsync(admin, adminPW);

                if (!addAdminResult.Succeeded)
                {
                    throw new Exception(string.Join("\n", addAdminResult.Errors));
                }

                var adminUser = await _userManager.FindByEmailAsync(TeacherEmail);

                foreach (var role in roleNames)
                {
                    if (await _userManager.IsInRoleAsync(adminUser, role))
                    {
                        continue;
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(adminUser, role);

                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("\n", addToRoleResult.Errors));
                    }
                }
                
                var courses = new List<Course>();
                
                for (var i = 0; i < 5; i++)
                {
                    var date = DateTime.Now.AddDays(_fake.Random.Int(-22, 0));
                    var course = new Course
                    {
                        Title = _fake.Company.CatchPhrase(),
                        Description = _fake.Hacker.Verb(),
                        StartDate = date,
                        EndDate = date.AddMonths(6),
                        Modules = GetModules(10),
                    };
                    courses.Add(course);
                }
                await context.AddRangeAsync(courses);
                await context.SaveChangesAsync();


                var students = GetStudents(20);
                
                foreach (var student in students)
                {
                    var addStudentResult = await _userManager.CreateAsync(student, studentPW);
                    
                    if (!addStudentResult.Succeeded)
                    {
                        throw new Exception(string.Join("\n", addStudentResult.Errors));
                    }

                    var studentUser = await _userManager.FindByEmailAsync(student.Email);

                    if (studentUser is null)
                    {
                        continue;
                    }

                    if (await _userManager.IsInRoleAsync(studentUser, roleNames[1]))
                    {
                        continue;
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(studentUser, roleNames[1]);

                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("\n", addToRoleResult.Errors));
                    }

                }
                
                var enrollmentsList = new List<ApplicationUserCourse>();
                foreach (var s in students)
                {
                        var junction = new ApplicationUserCourse
                        {
                            ApplicationUserId = s.Id,
                            CourseId = courses[_fake.Random.Int(0,4)].Id
                        };
                    enrollmentsList.Add(junction);
                    
                }
                await context.AddRangeAsync(enrollmentsList);
                await context.SaveChangesAsync();
                var randomInt = _fake.Random.Int(1, 5);
                foreach (var c in courses)
                {
                    c.Documents = GetDocuments(randomInt, students);
                    foreach (var m in c.Modules)
                    {
                        m.Documents = GetDocuments(randomInt, students);

                        foreach (var a in m.Activities)
                        {
                            a.Documents = GetDocuments(randomInt, students);
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
        
        private static List<Module> GetModules(int count)
        {
            var modules = new List<Module>();
            for (var i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(_fake.Random.Int(-12, 2));
                var module = new Module
                {
                    Title = _fake.Company.CatchPhrase(),
                    Description = _fake.Commerce.ProductAdjective(),
                    StartDate = date,
                    EndDate = date.AddMonths(1),
                    Activities = GetActivities(5),
                };
                modules.Add(module);
            }
            return modules;
        }

        private static List<Activity> GetActivities(int count)
        {
            var activities = new List<Activity>();
            for (var i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(_fake.Random.Int(-2, 2));

                var activity = new Activity
                {
                    Name = _fake.Lorem.Word(),
                    Description = _fake.Hacker.Verb(),
                    StartDate = date,
                    EndDate = date.AddDays(5),
                    ActivityType = _fake.PickRandom<ActivityType>(),
                };

                activities.Add(activity);
            }

            return activities;
        }

        private static List<Document> GetDocuments(int count, List<ApplicationUser> listStudent)
        {
            var documents = new List<Document>();
            for (var i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(_fake.Random.Int(-12, -2));

                var document = new Document
                {
                    Name = _fake.System.CommonFileName(),
                    Description = _fake.Hacker.Verb(),
                    UploadTime = date,
                    UserId = listStudent[_fake.Random.Int(1, (listStudent.Count-1))].Id
                };

                documents.Add(document);
            }
            return documents;
        }

        private static List<ApplicationUser> GetStudents(int count)
        {
            var list = new List<ApplicationUser>();
            for (var i = 0; i < count; i++)
            {
                var fName = _fake.Name.FirstName();
                var lName = _fake.Name.LastName();
                var student = new ApplicationUser()
                {
                    FirstName = fName,
                    LastName = lName,
                    UserName = fName,
                    Email = _fake.Internet.Email($"{fName} {lName}")
                };
                list.Add(student);
            }
            return list;
        }
    }
}
