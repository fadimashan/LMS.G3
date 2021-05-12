using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;
using LMS.Data.Data;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class ModulesController : Controller
    {
        private readonly LMSWebContext db;

        public ModulesController(LMSWebContext context)
        {
            db = context;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            return View(await db.Module.ToListAsync());
        }

        // GET: Modules/Details/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!db.Module.Any(m => m.Id == id))
            {
                return NotFound();
            }

            var @module = await db.Module.Include(m=> m.Activities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Modules/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            var model = new Module
            {
                GetAllCourses = GetAllCourses()
            };

            return View(model);
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartDate,EndDate,CourseId")] Module @module)
        {
            if (ModelState.IsValid)
            {
                db.Add(@module);
                await db.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));
                return Redirect("/courses");

            }
            //return View(@module);
            return Redirect("/courses");
        }

        // GET: Modules/Edit/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await db.Module.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,StartDate,EndDate")] Module @module)
        {
            var mod = db.Module.Find(id);

            if (mod == null) { return NotFound(); }

            if (ModelState.IsValid)
            {
                try
                {
                    mod.Title = module.Title;
                    mod.StartDate = module.StartDate;
                    mod.EndDate = module.EndDate;
                    mod.Description = module.Description;
                    db.Update(mod);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return Redirect($"/modules/details/{id}");

            }
            return View(@module);
        }

        // GET: Modules/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await db.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @module = await db.Module.FindAsync(id);
            db.Module.Remove(@module);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return db.Module.Any(e => e.Id == id);
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


        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateFromModule(int id)
        {
           
            var @module = await db.Module.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }

            var activity = new Activity()
            {
                ModuleId = id
            };
            return View(activity);
        }

        // POST: Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromModule(int id,[Bind("Name,ActivityType,StartDate,EndDate,Description,ModuleId")] Activity activity)
        {

            var mod = db.Module.Find(id);
            if (ModelState.IsValid)
            {
               // mod.Activities.Add(activity);
                db.Add(activity);
                await db.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return Redirect($"/modules/details/{activity.ModuleId}");

            }
            //return View(activity);
            return Redirect("/courses");
        }

    }
}
