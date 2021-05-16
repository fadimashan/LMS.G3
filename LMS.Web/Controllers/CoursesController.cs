using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;
using LMS.Core.Entities.ViewModels;
using LMS.Data.Data;
using static LMS.Web.Areas.Identity.Pages.Account.LoginModel;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly MvcDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(MvcDbContext context, UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        // GET: Courses
        public async Task<IActionResult> GetCourses()
        {
            var module = await _dbContext.Course
                .Include(c => c.Modules)
                .ToListAsync();

            return View("GetCourses", module);
        }

        // Check for Course Unique Title
        public IActionResult VerifyName(Course course)
        {
          
            if (_dbContext.Course.Any(c => c.Title.ToUpper() == course.Title.ToUpper()))
            {
                return Json("Course Title must be uppercase and unique");
            }
            return Json(true);
        }

        // Check for email exist
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var foundEmail = await _dbContext.Users.ToListAsync();

            var newList = new List<bool>();
            foreach (var s in foundEmail)
            {
                newList.Add(s.Email == email);
            }

            var result = newList.Contains(true);

            if (result) // is it wrong concept?
            {
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }
        
        // Check for user FirstName and LastName
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyName(NewUserViewModel model, string firstName, string lastName)
        {
            if(model.FirstName == lastName || model.LastName == firstName)
            {
                return Json($"First name: {firstName}, must not be the same with Last Name: {lastName}");
            }
            var foundEmail = _dbContext.Users.ToList();

            if (foundEmail.Any(a => a.FirstName == firstName && a.LastName == lastName))
            {
                return Json($"This name: {firstName}, is already in the system ");
            }

            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPassword(NewUserViewModel model)
        {
            if (model.Password != model.ConfirmPassword && !String.IsNullOrEmpty(model.ConfirmPassword) )
            {
                return Json($"Not matching with Confirm Password!");
            }

            var symbols = (@"[!@#$%^&*]");
            var list = new List<bool>();
            foreach (var char1 in symbols)
            {
                list.Add(model.Password.Contains(char1));
            }

            if (!list.Contains(true))
            {
                return Json($"Password should contains one of this symbols !@#$%^&*");

            }
            return Json(true);
        }
        
        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var course = await _dbContext.Course
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course is null)
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
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartDate,EndDate")] Course course, [Bind("Id,Title,Description,StartDate,EndDate")] Module module)
        {
            if (ModelState.IsValid)
            {
                List<Module> modules = new List<Module>();
                modules.Add(module);
                var newCourse = course;
                newCourse.Modules = modules;
                await _dbContext.AddAsync(newCourse);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // return View(course);
            // return View("GetCourses", course);
            return View("Details", course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var course = await _dbContext.Course.FindAsync(id);

            if (course is null)
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
                    _dbContext.Update(course);
                    await _dbContext.SaveChangesAsync();
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
            if (id is null)
            {
                return NotFound();
            }

            var course = await _dbContext.Course
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course is null)
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
            var course = await _dbContext.Course.FindAsync(id);
            _dbContext.Course.Remove(course);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _dbContext.Course.Any(c => c.Id == id);
        }
        
        // public async Task<IActionResult> UserMainPageViewModel()
        public async Task<IActionResult> Index(string moduleID)
        {
            var currentUser = _userManager.GetUserId(User);
            ViewBag.CurrentModule = moduleID;
            int modID = 1;
            var ne = int.TryParse(moduleID, out modID);
            var courses = new List<Course>();

            if (User.IsInRole("Teacher"))
            {
                var modules = await _dbContext.Course
                    .Include(c => c.Documents)
                    .Include(c => c.Students)
                    .Include(c => c.Modules)
                    .ThenInclude(m=>m.Documents)
                    .Include(m=> m.Modules)
                    .ThenInclude(m => m.Activities)
                    .ThenInclude(a => a.Documents)
                    .ToListAsync();
                
                return View("GetCourses", modules);
                // return Redirect("/courses/GetCourses);
            }

            if (moduleID is null && User.IsInRole("Student"))
            {
                var firstCourseID = _dbContext.Course
                    .Where(c => c.Students.Any(au => au.Id == currentUser))
                    .Include(c => c.Modules)
                    .FirstOrDefault();
                courses = await _dbContext.Course
                    .Where(c => c.Students.Any(au => au.Id == currentUser))
                    .Include(c => c.Students)
                    .Include(c => c.Modules)
                    .ThenInclude(m => m.Activities
                        .Where(a => a.ModuleId == firstCourseID.Modules.FirstOrDefault().Id))
                    .ToListAsync();
            }
            else if (User.IsInRole("Student"))
            {
                courses = await _dbContext.Course
                    .Where(c => c.Students.Any(au => au.Id == currentUser))
                    .Include(c => c.Students)
                    .Include(c => c.Modules)
                    .ThenInclude(m => m.Activities
                        .Where(a => a.ModuleId == modID))
                    .ToListAsync();
            }

            if (User.IsInRole("Student"))
            {
                return View("UserMainPageViewModel", courses);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> GetStudents()
        {
            var currentUserId = _userManager.GetUserId(User);
            var course = await _dbContext.Course
                .Where(c => c.Students.Any(au => au.Id == currentUserId))
                .Include(c => c.Students)
                .FirstOrDefaultAsync();

            return View("GetStudentsForThisCourse", course);
        }

        public async Task<IActionResult> GetAllStudents(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                var students = await _dbContext.Course
                .Include(c => c.Students)
                .ToListAsync();

                return View("GetAllStudents", students);
            } 
            else
            {
                var students = await _dbContext.Course
           .Where(c => c.Students.Any(s => s.LastName.ToLower().StartsWith(name.ToLower()) || s.FirstName.ToLower().StartsWith(name.ToLower()) || name == null))
           .Include(c => c.Students)
           .ToListAsync();
                return View("GetAllStudents", students);
            }
        }


      


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("GetAllStudents");
                }
                else
                {
                    return NotFound(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            // FIXME: Value `_userManager.Users` is not assignable to model `IEnumerable<Course>`
            return View("GetAllStudents");
        }

        public IActionResult AddUser()
        {
            var model = new NewUserViewModel
            {
                GetAllCourses = GetAllCourses()
            };
            return View("AddUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddUser([Bind("FirstName,LastName,Email,Password,RoleType,CourseId")] NewUserViewModel userVM)
        {
            var newUser = new ApplicationUser()
            {
                FirstName = userVM.FirstName,
                LastName = userVM.LastName,
                UserName = userVM.FirstName,
                Email = userVM.Email,
            };
            
            var type = userVM.RoleType == "A" ? "Student" : "Teacher";

            var addStudentResult = await _userManager.CreateAsync(newUser, userVM.Password);

            if (!addStudentResult.Succeeded)
            {
                throw new Exception(string.Join("\n", addStudentResult.Errors));
            }

            var userFromManager = await _userManager.FindByEmailAsync(newUser.Email);

            if (userFromManager is null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(userFromManager, type))
            {
                return NotFound();
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(userFromManager, type);

            if (!addToRoleResult.Succeeded)
            {
                throw new Exception(string.Join("\n", addToRoleResult.Errors));
            }
            
            if (type == RoleType.Student.ToString())
            {
                var enrol = new ApplicationUserCourse
                {
                    ApplicationUserId = userFromManager.Id,
                    CourseId = userVM.CourseId
                };
                await _dbContext.AddAsync(enrol);
            }
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(GetAllStudents));
        }

        public IEnumerable<SelectListItem> GetAllCourses()
        {
            var courses = new List<SelectListItem>();

            foreach (var course in _dbContext.Course.ToList())
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
            var courses = _dbContext.Course
                    .Include(c => c.Modules)
                    .Include(s => s.Students)
                    .Include(d => d.Documents)
                    .ToList();
            // Variable `course` is never used
            var course = _dbContext.Course.Find(id);
            var userId = _userManager.GetUserId(User);
            if (files is not null && files.Length > 0)
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
                    _dbContext.Document.Add(doc);

                }
                _dbContext.SaveChanges();
            }
            else
            {
                ViewBag.Message = "Please select a doc.";
                return View("GetCourses", courses);
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
            
            return View("GetCourses", courses);
            //return Redirect("/Courses");
        }
    }
}
