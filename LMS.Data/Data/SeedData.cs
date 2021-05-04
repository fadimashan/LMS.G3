using Bogus;
using LMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Data
{
    public static class SeedData
    {
        private static Faker fake;
        private static UserManager<ApplicationUser> userManager;
        private static RoleManager<IdentityRole> roleManager;

        public static async Task InitAsync(IServiceProvider services, string adminPW, string studentPW)
        {
            using (var context = new LMSWebContext(services.GetRequiredService<DbContextOptions<LMSWebContext>>()))
            {

                userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


                fake = new Faker("sv");

                var roleNames = new[] { "Teacher", "Student" };

                foreach (var roleName in roleNames)
                {
                    if (await roleManager.RoleExistsAsync(roleName)) continue;

                    var role = new IdentityRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
                }


                var TeacherEmail = "dimitris@lms.se";

                var foundTeacher = await userManager.FindByEmailAsync(TeacherEmail);

                if (foundTeacher != null) return;

                var admin = new ApplicationUser
                {
                    UserName = "dimitris",
                    Email = TeacherEmail,
                };

                var addAdminResult = await userManager.CreateAsync(admin, adminPW);

                if (!addAdminResult.Succeeded) throw new Exception(string.Join("\n", addAdminResult.Errors));

                var adminUser = await userManager.FindByEmailAsync(TeacherEmail);

                foreach (var role in roleNames)
                {
                    if (await userManager.IsInRoleAsync(adminUser, role)) continue;

                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, role);

                    if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
                }



                var courses = new List<Course>();
                for (int i = 0; i < 5; i++)
                {
                    var date = DateTime.Now;
                    var course = new Course
                    {
                        Title = fake.Company.CatchPhrase(),
                        Description = fake.Hacker.Verb(),
                        StartDate = date,
                        EndDate = date.AddMonths(6),
                        Modules = GetModules(10),
                        //Students = GetStudents(30)
                    };
                    courses.Add(course);
                }
                context.AddRange(courses);
                context.SaveChanges();


                var students = GetStudents(20);
                
                foreach (var student in students)
                {
                    var addStudentResult = await userManager.CreateAsync(student, studentPW);
                    if (!addStudentResult.Succeeded) throw new Exception(string.Join("\n", addStudentResult.Errors));

                    var studentUser = await userManager.FindByEmailAsync(student.Email);

                    if (studentUser is null) continue;
                    if (await userManager.IsInRoleAsync(studentUser, roleNames[1])) continue;

                    var addToRoleResult = await userManager.AddToRoleAsync(studentUser, roleNames[1]);

                    if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));

                }


                var enrollist = new List<ApplicationUserCourse>();
                foreach (var s in students)
                {
                        var junktion = new ApplicationUserCourse
                        {
                            ApplicationUserId = s.Id,
                            CourseId = courses[fake.Random.Int(0,4)].Id
                        };
                    enrollist.Add(junktion);
                    
                }
                context.AddRange(enrollist);
                context.SaveChanges();
                //var newStudentList = new List<ApplicationUser>();
                var randomInt = fake.Random.Int(1, 5);
                foreach (var c in courses)
                {
                    //for (int i = 0; i < 20; i++)
                    //{
                    //    var student1 = students[fake.Random.Int(0, 49)];
                    //    if (c.Students is null || !c.Students.Contains(student1))
                    //    {
                    //        newStudentList.Add(student1);
                    //    }
                    //}
                    //c.Students = newStudentList;
                    //c.Documents = GetDocuments(randomInt, c.Students.ToList());
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
                context.SaveChanges();
                await context.SaveChangesAsync();


            }
        }


        private static List<Module> GetModules(int count)
        {
            var modules = new List<Module>();
            for (int i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(fake.Random.Int(-2, 2));

                var module = new Module
                {
                    Title = fake.Company.CatchPhrase(),
                    Description = fake.Commerce.ProductAdjective(),
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
            for (int i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(fake.Random.Int(-2, 2));

                var activity = new Activity
                {
                    Name = fake.Lorem.Word(),
                    Description = fake.Hacker.Verb(),
                    StartDate = date,
                    EndDate = date.AddDays(5),
                    ActivityType = fake.PickRandom<ActivityType>(),
                };

                activities.Add(activity);
            }

            return activities;
        }

        private static List<Document> GetDocuments(int count, List<ApplicationUser> listStudent)
        {
            var documents = new List<Document>();
            for (int i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(fake.Random.Int(-12, -2));

                var document = new Document
                {
                    Name = fake.System.CommonFileName(),
                    Description = fake.Hacker.Verb(),
                    UploadTime = date,
                    UserId = listStudent[fake.Random.Int(1, (listStudent.Count-1))].Id
                };

                documents.Add(document);
            }
            return documents;
        }

        private static List<ApplicationUser> GetStudents(int count)
        {
            var list = new List<ApplicationUser>();
            for (int i = 0; i < count; i++)
            {
                var fName = fake.Name.FirstName();
                var lName = fake.Name.LastName();
                var student = new ApplicationUser()
                {
                    FirstName = fName,
                    LastName = lName,
                    UserName = fName,
                    Email = fake.Internet.Email($"{fName} {lName}")

                };
                list.Add(student);
            }
            return list;
        }
    }
}
