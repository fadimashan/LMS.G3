using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;
using LMS.Core.Entities.ViewModels;
using LMS.Data.Data;

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
            var module = await _dbContext.Course.Include(c => c.Modules).ToListAsync();
            return View("GetCourses", module);
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
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartDate,EndDate")] Course course, [Bind("Id,Title,Description,StartDate,EndDate")] Module @module)
        {
            if (ModelState.IsValid)
            {
                List<Module> newMod = new List<Module>();
                newMod.Add(module);
                var co = course;
                co.Modules = newMod;
                _dbContext.Add(co);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            var course = new List<Course>();

            if (User.IsInRole("Teacher"))
            {
                var modules = await _dbContext.Course
                    .Include(c => c.Students)
                    .Include(c => c.Modules)
                    .ThenInclude(m => m.Activities)
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
                course = await _dbContext.Course
                    .Where(c => c.Students.Any(au => au.Id == currentUser))
                    .Include(s=> s.Students)
                    .Include(c => c.Modules)
                    .ThenInclude(m => 
                        m.Activities.Where(a => a.ModuleId == firstCourseID.Modules.FirstOrDefault().Id))
                    .ToListAsync();
            }
            else if (User.IsInRole("Student"))
            {
                course = await _dbContext.Course
                    .Where(c => c.Students.Any(e => e.Id == currentUser))
                    .Include(s => s.Students)
                    .Include(c => c.Modules)
                    .ThenInclude(m => 
                        m.Activities.Where(a => a.ModuleId == modID))
                    .ToListAsync();
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
            var course = await _dbContext.Course
                .Where(c => c.Students.Any(e => e.Id == currentUser))
                .Include(c => c.Students)
                .FirstOrDefaultAsync();

            return View("GetStudentsForThisCourse", course);
        }

        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _dbContext.Course
                .Include(c => c.Students)
                .ToListAsync();

            return View("GetAllStudents", students);
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
            return View("GetAllStudents", _userManager.Users);
        }

        public IActionResult AddUser()
        {
            var model = new NewUserViewModel
            {
                GetAllCourses = GetAllCourses()
            };
            return View("AddUser",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddUser([Bind("FirstName,LastName,Email,Password,RoleType,CourseId")] NewUserViewModel user)
        {
            var newUser = new ApplicationUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.FirstName,
                Email = user.Email,
            };
            var type = user.RoleType == "A" ? "Student" : "Teacher"; 

            var addStudentResult = await _userManager.CreateAsync(newUser, user.Password);

            if (!addStudentResult.Succeeded)
            {
                throw new Exception(string.Join("\n", addStudentResult.Errors));
            }

            var userFromManager = await _userManager.FindByEmailAsync(newUser.Email);

            if (userFromManager is null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(userFromManager, user.RoleType))
            {
                return NotFound();
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(userFromManager, user.RoleType);

            if (!addToRoleResult.Succeeded)
            {
                throw new Exception(string.Join("\n", addToRoleResult.Errors));
            }


            if (user.RoleType == RoleType.Student.ToString())
            {
                var enrol = new ApplicationUserCourse
                {
                    ApplicationUserId = userFromManager.Id,
                    CourseId = user.CourseId
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
    }
}
