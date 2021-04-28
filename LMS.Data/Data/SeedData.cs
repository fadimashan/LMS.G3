﻿using Bogus;
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
                var listOfStudent = new List<ApplicationUser>();

                fake = new Faker("sv");

                var courses = GetCourses(2);
                var modules = GetModules(20);
                var activities = GetActivities(10);
                var documents = GetDocuments(20, listOfStudent);

                foreach (var course in courses)
                {
                    var someModules = new List<Module>();
                    var someDocs = new List<Document>(); ;
                    var r = new Random();

                    for (int i = 0; i < 5; i++)
                    {
                        someModules.Add(modules[r.Next(0, 5)]);
                        someDocs.Add(documents[r.Next(0, 20)]);
                    }
                    course.Modules = someModules;
                    course.Documents = someDocs;
                }

                foreach (var module in modules)
                {
                    var someActivites = new List<Activity>();
                    var someDocs = new List<Document>(); ;
                    var r = new Random();

                    for (int i = 0; i < 5; i++)
                    {
                        someActivites.Add(activities[r.Next(0, 5)]);
                        var doc = fake.PickRandom(documents);
                        if (courses.Any(c => c.Documents.Contains(doc))) continue;
                        someDocs.Add(doc);
                    }
                    module.Activities = someActivites;
                    module.Documents = someDocs;
                }


                foreach (var activty in activities)
                {
                    var someDocs = new List<Document>(); ;
                    var r = new Random();

                    for (int i = 0; i < 5; i++)
                    {
                        var doc = fake.PickRandom(documents);
                        if (courses.Any(c => c.Documents.Contains(doc))) continue;
                        if (modules.Any(c => c.Documents.Contains(doc))) continue;
                        someDocs.Add(doc);
                    }
                    activty.Documents = someDocs;
                }



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


                for (int i = 0; i < 20; i++)
                {
                    var studentName = fake.Internet.UserName();
                    var Email = fake.Internet.Email();
                    var student = new ApplicationUser()
                    {
                        UserName = studentName,
                        Email = Email
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
                await context.AddRangeAsync(courses);
                await context.AddRangeAsync(modules);
                await context.AddRangeAsync(activities);
                await context.AddRangeAsync(documents);
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

        private static List<Module> GetModules(int count)
        {
            var modules = new List<Module>();
            for (int i = 0; i < count; i++)
            {
                var date = DateTime.Now.AddDays(fake.Random.Int(-2, 2));

                var module = new Module
                {
                    Title = fake.Company.CatchPhrase(),
                    Description = fake.Hacker.Verb(),
                    StartDate = date,
                    EndDate = date.AddMonths(1)
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
                var date = DateTime.Now.AddDays(fake.Random.Int(-2, -12));

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
    }
}
