using LMS.Core.Entities;
using LMS.Core.Entities.ViewModels;
using LMS.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly LMSWebContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(LMSWebContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        // GET: Courses
        public async Task<IActionResult> GetCourses()
        {
            var module = await db.Course.Include(c => c.Modules).ToListAsync();
            return View("GetCourses", module);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await db.Course
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartDate,EndDate")] Course course, [Bind("Id,Title,Description,StartDate,EndDate")] Module @module)
        {
            if (ModelState.IsValid)
            {
                List<Module> newMod = new List<Module>();
                newMod.Add(module);
                var co = course;
                co.Modules = newMod;
                db.Add(co);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("GetCourses", course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await db.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartDate,EndDate")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(course);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await db.Course
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await db.Course.FindAsync(id);
            db.Course.Remove(course);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return db.Course.Any(e => e.Id == id);
        }


        // public async Task<IActionResult> UserMainPageViewModel()
        public async Task<IActionResult> Index(string moduleID)
        {
            var currentUser = _userManager.GetUserId(User);
            ViewBag.CurrentModule = moduleID;
            int modID = 1;
            var ne = int.TryParse(moduleID, out modID);
            var course = new List<Course>();

            if (User.IsInRole("Teacher"))
            {
                var module = await db.Course.Include(c => c.Documents).Include(c => c.Students).Include(c => c.Modules).ThenInclude(m=>m.Documents).Include(m=> m.Modules).ThenInclude(m => m.Activities).ThenInclude(a => a.Documents).ToListAsync();
                return View("GetCourses", module);
            }

            if (moduleID is null && User.IsInRole("Student"))
            {
                var firstModuleID = db.Course.Where(c => c.Students.Any(e => e.Id == currentUser))
                .Include(c => c.Modules).FirstOrDefault();
                course = await db.Course.Where(c => c.Students.Any(e => e.Id == currentUser)).Include(s => s.Students)
                   .Include(c => c.Modules).ThenInclude(m => m.Activities.Where(a => a.ModuleId == firstModuleID.Modules.FirstOrDefault().Id)).ToListAsync();
            }
            else if (User.IsInRole("Student"))
            {
                course = await db.Course.Where(c => c.Students.Any(e => e.Id == currentUser)).Include(s => s.Students)
            .Include(c => c.Modules).ThenInclude(m => m.Activities.Where(a => a.ModuleId == modID)).ToListAsync();
            }

            if (User.IsInRole("Student"))
            {
                return View("UserMainPageViewModel", course);
            }
            else
            {
                return NotFound();
            }

        }

        public async Task<IActionResult> GetStudents()
        {
            var currentUser = _userManager.GetUserId(User);
            var course = await db.Course.Where(c => c.Students.Any(e => e.Id == currentUser))
                .Include(c => c.Students).FirstOrDefaultAsync();

            return View("GetStudentsForThisCourse", course);
        }

        public async Task<IActionResult> GetAllStudents()
        {
            var students = await db.Course
                .Include(c => c.Students).ToListAsync();

            return View("GetAllStudents", students);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("GetAllStudents");
                else
                    return NotFound(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("GetAllStudents", _userManager.Users);
        }


        public IActionResult AddUser()
        {
            var model = new NewUserViewModule
            {
                GetAllCourses = GetAllCourses()
            };
            return View("AddUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddUser([Bind("FirstName,LastName,Email,Password,RoleType,CourseId")] NewUserViewModule user)
        {
            var user1 = new ApplicationUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.FirstName,
                Email = user.Email,

            };
            var type = user.RoleType.ToString() == "A" ? "Student" : "Teacher";

            var addStudentResult = await _userManager.CreateAsync(user1, user.Password);
            if (!addStudentResult.Succeeded) throw new Exception(string.Join("\n", addStudentResult.Errors));

            var newUser = await _userManager.FindByEmailAsync(user1.Email);

            if (newUser is null) return NotFound();
            if (await _userManager.IsInRoleAsync(newUser, user.RoleType.ToString())) return NotFound();

            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, user.RoleType.ToString());

            if (!addToRoleResult.Succeeded) throw new Exception(string.Join("\n", addToRoleResult.Errors));


            if (user.RoleType.ToString() == RoleType.Student.ToString())
            {
                var enrol = new ApplicationUserCourse
                {
                    ApplicationUserId = newUser.Id,
                    CourseId = user.CourseId
                };
                db.Add(enrol);
            }
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(GetAllStudents));
        }

        public IEnumerable<SelectListItem> GetAllCourses()
        {
            var courses = new List<SelectListItem>();

            foreach (var course in db.Course.ToList())
            {
                var selectListItem = (new SelectListItem
                {
                    Text = course.Title,
                    Value = course.Id.ToString()

                });
                courses.Add(selectListItem);
            }
            return (courses);

        }


        [HttpPost]
        public IActionResult UploadCourseDocument(int id, IFormFile[] files)
        {
            var course = db.Course.Find(id);
            var userId = _userManager.GetUserId(User);
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {

                    var fileName = System.IO.Path.GetFileName(file.FileName);


                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var localFile = System.IO.File.OpenWrite(filePath))
                    using (var uploadedFile = file.OpenReadStream())
                    {
                        uploadedFile.CopyTo(localFile);
                    }

                    var doc = new Document()
                    {
                        Name = fileName,
                        CourseId = id,
                        UploadTime = DateTime.Now,
                        Description = filePath,
                        UserId = userId
                    };
                    db.Document.Add(doc);

                }
                db.SaveChanges();
            }

            var model = new FilesViewModel();
            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files")))
            {
                model.Files.Add(
                    new FileDetails
                    {
                        Name = System.IO.Path.GetFileName(item),
                        UserName = User.Identity.Name,
                        UploadTime = DateTime.Now,
                        CourseId = id,
                        UserId = userId,
                        Path = item
                    });
            }

            return Redirect("/courses");

        }

    }
}
