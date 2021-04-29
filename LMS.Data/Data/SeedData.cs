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


        public static async Task InitAsync(IServiceProvider services, string adminPW, string studentPW)
        {
            using (var context = new LMSWebContext(services.GetRequiredService<DbContextOptions<LMSWebContext>>()))
            {

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


                fake = new Faker("sv");

                var date = DateTime.Now;
                var course45 = new Course
                {
                    Title = fake.Company.CatchPhrase(),
                    Description = fake.Hacker.Verb(),
                    StartDate = date,
                    EndDate = date.AddMonths(6),
                    Modules = GetModules2(10),

                };

                context.Add(course45);
                context.SaveChanges();


                var courses = GetCourses(2);
                context.AddRange(courses);
                context.SaveChanges();

               var modules = GetModules(20, 1);
                var activities = GetActivities(40);

                var roleNames = new[] { "Teacher", "Student" };

                foreach (var roleName in roleNames)
                {
                    if (await roleManager.RoleExistsAsync(roleName)) continue;

                    var role = new IdentityRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
                }

                var listOfStudent = new List<ApplicationUser>();
                //CreateAdmin(userManager, roleNames, adminPW);

                //if (!context.Users.Any())
                //{
                //    listOfStudent = CreateUsers();
                //}


                //var TeacherEmail = "dimitris@lms.se";

                //var foundTeacher = await userManager.FindByEmailAsync(TeacherEmail);

                //if (foundTeacher != null) return;

                //var admin = new ApplicationUser
                //{
                //    UserName = "dimitris",
                //    Email = TeacherEmail,
                //};

                //var addAdminResult = await userManager.CreateAsync(admin, adminPW);

                //if (!addAdminResult.Succeeded) throw new Exception(string.Join("\n", addAdminResult.Errors));

                //var adminUser = await userManager.FindByEmailAsync(TeacherEmail);

                //foreach (var role in roleNames)
                //{
                //    if (await userManager.IsInRoleAsync(adminUser, role)) continue;

                //    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, role);

                //    if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
                //}


                for (int i = 0; i < 5; i++)
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
                    listOfStudent.Add(student);
                }

                foreach (var student in listOfStudent)
                {
                    var addStudentResult = await userManager.CreateAsync(student, studentPW);
                    if (!addStudentResult.Succeeded) throw new Exception(string.Join("\n", addStudentResult.Errors));

                    var studentUser = await userManager.FindByEmailAsync(student.Email);

                    if (await userManager.IsInRoleAsync(studentUser, roleNames[1])) continue;

                    var addToRoleResult = await userManager.AddToRoleAsync(studentUser, roleNames[1]);

                    if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
                }
                //await context.SaveChangesAsync();
                //if (listOfStudent.Any())
                //{
                // CreateStudentRoles(userManager, roleNames, studentPW, listOfStudent);

                var documents = GetDocuments(20, listOfStudent);
                //var allmodules = new List<Module>();
                //for (int i = 0; i < modules.Count; i++)
                //{
                //    allmodules.Add(modules[i]);
                //}

                foreach (var course in courses)
                {
                   
                    course.Modules = GetModules(5, course.Id);
                   // course.Documents = ;
                }

               // await context.AddRangeAsync(courses);
                

                await context.SaveChangesAsync();

                var allActivities = new List<Activity>();
                for (int i = 0; i < activities.Count; i++)
                {
                    allActivities.Add(activities[i]);
                }

                foreach (var module in modules)
                {
                    var someActivites = new List<Activity>();
                    var someDocs = new List<Document>(); ;
                    var r = new Random();

                    for (int i = 0; i < 2; i++)
                    {
                        var activty = allActivities[0];
                        if (!someActivites.Contains(activty))
                        {
                            someActivites.Add(activty);
                            allActivities.Remove(activty);
                        }
                        var doc = fake.PickRandom(documents);
                        if (courses.Any(c => c.Documents.Contains(doc))) continue;
                        someDocs.Add(doc);
                    }
                    module.Activities = someActivites;
                    module.Documents = someDocs;
                }

                await context.AddRangeAsync(modules);
                await context.SaveChangesAsync();


                foreach (var activty in activities)
                {
                    var someDocs = new List<Document>(); ;
                    var r = new Random();

                    for (int i = 0; i < documents.Count; i++)
                    {
                        var doc = documents[i];
                        if (!courses.Any(c => c.Documents.Contains(doc)) || !modules.Any(c => c.Documents.Contains(doc)))
                        {
                            if (!someDocs.Contains(doc)) someDocs.Add(doc);
                        }
                    }
                    activty.Documents = someDocs;
                }
                await context.AddRangeAsync(activities);
                await context.SaveChangesAsync();

                await context.AddRangeAsync(documents);
                await context.SaveChangesAsync();

                //}
                var enrollments = new List<ApplicationUserCourse>();
                foreach (var user in context.Users)
                {
                    if (await userManager.IsInRoleAsync(user, "Student"))
                    {
                        foreach (var course in courses)
                        {
                            if (course == courses[0])
                            {
                                var enrollment = new ApplicationUserCourse
                                {
                                    CourseId = course.Id,
                                    ApplicationUserId = user.Id

                                };
                                enrollments.Add(enrollment);
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    if (await userManager.IsInRoleAsync(user, "Teacher"))
                    {
                        foreach (var course in courses)
                        {

                            var enrollment = new ApplicationUserCourse
                            {
                                Course = course,
                                Student = user

                            };
                            enrollments.Add(enrollment);

                        }
                    }
                }

              

                await context.AddRangeAsync(enrollments);
                await context.SaveChangesAsync();
            }
        }



        private static List<Course> GetCourses(int count)
        {
            var courses = new List<Course>();

            for (int i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(fake.Random.Int(-2, 2));
                var course = new Course
                {
                    Title = fake.Company.CatchPhrase(),
                    Description = fake.Hacker.Verb(),
                    StartDate = date,
                    EndDate = date.AddMonths(6)
                };

                courses.Add(course);
            }

            return courses;
        }

        private static List<Module> GetModules(int count, int courceId)
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
                    CourseId = courceId
                };
                modules.Add(module);
            }
            return modules;
        } 
        
        private static List<Module> GetModules2(int count)
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
                    Activities = GetActivities(5)
                    
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
                    ActivityType = fake.PickRandom<ActivityType>()
                };

                activities.Add(activity);
            }

            return activities;
        }

        private static List<Document> GetDocuments(int count, List<ApplicationUser> listOfStudent)
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
                    ApplicationUser = fake.PickRandom<ApplicationUser>(listOfStudent),
                };

                documents.Add(document);
            }
            return documents;
        }

        //private static async void CreateAdmin(UserManager<ApplicationUser> userManager, string[] roleNames, string adminPW)
        //{
        //    var TeacherEmail = "dimitris@lms.se";

        //    var foundTeacher = await userManager.FindByEmailAsync(TeacherEmail);

        //    if (foundTeacher != null) return;

        //    var admin = new ApplicationUser
        //    {
        //        UserName = "dimitris",
        //        Email = TeacherEmail,
        //    };

        //    var addAdminResult = await userManager.CreateAsync(admin, adminPW);

        //    if (!addAdminResult.Succeeded) throw new Exception(string.Join("\n", addAdminResult.Errors));

        //    var adminUser = await userManager.FindByEmailAsync(TeacherEmail);

        //    foreach (var role in roleNames)
        //    {
        //        if (await userManager.IsInRoleAsync(adminUser, role)) continue;

        //        var addToRoleResult = await userManager.AddToRoleAsync(adminUser, role);

        //        if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
        //    }
        //}

        //private static List<ApplicationUser> CreateUsers()
        //{
        //    var listOfStudent = new List<ApplicationUser>();

        //    for (int i = 0; i < 20; i++)
        //    {
        //        var studentName = fake.Internet.UserName();
        //        var Email = fake.Internet.Email();
        //        var student = new ApplicationUser()
        //        {
        //            UserName = studentName,
        //            Email = Email

        //        };
        //        listOfStudent.Add(student);
        //    }

        //    return listOfStudent;
        //}

        //private static async void CreateStudentRoles(UserManager<ApplicationUser> userManager, string[] roleNames, string studentPW, List<ApplicationUser> listOfStudent)
        //{

        //    foreach (var student in listOfStudent)
        //    {
        //        var addStudentResult = await userManager.CreateAsync(student, studentPW);
        //        if (!addStudentResult.Succeeded) throw new Exception(string.Join("\n", addStudentResult.Errors));

        //        var studentUser = await userManager.FindByEmailAsync(student.Email);

        //        if (await userManager.IsInRoleAsync(studentUser, roleNames[1])) continue;

        //        var addToRoleResult = await userManager.AddToRoleAsync(studentUser, roleNames[1]);

        //        if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));
        //    }

        //}
    }
}
