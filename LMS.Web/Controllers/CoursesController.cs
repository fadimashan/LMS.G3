using LMS.Core.Entities;
using LMS.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            return View(course);
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
                var module = await db.Course.Include(c => c.Modules).ThenInclude(m => m.Activities).ToListAsync();
                return View("GetCourses", module);

            }

            if (moduleID is null && User.IsInRole("Student"))
            {
                var firstModuleID = db.Course.Where(c => c.Students.Any(e => e.Id == currentUser))
                .Include(c => c.Modules).FirstOrDefault();
                course = await db.Course.Where(c => c.Students.Any(e => e.Id == currentUser))
                   .Include(c => c.Modules).ThenInclude(m => m.Activities.Where(a => a.ModuleId == firstModuleID.Modules.FirstOrDefault().Id)).ToListAsync();
            }
            else if(User.IsInRole("Student"))
            {
                course = await db.Course.Where(c => c.Students.Any(e => e.Id == currentUser))
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


    }
}
