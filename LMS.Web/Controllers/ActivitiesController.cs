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
    public class ActivitiesController : Controller
    {
        private readonly LMSWebContext db;

        public ActivitiesController(LMSWebContext context)
        {
            db = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            return View(await db.Activity.ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await db.Activity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // GET: Activities/Create
        public IActionResult Create()
        {
            var module = new Activity
            {
                GetModulesSelectListItem = GetModulesSelectListItem()
            };
            return View(module);
        }

        // POST: Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ActivityType,StartDate,EndDate,Description,ModuleId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Add(activity);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await db.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ActivityType,StartDate,EndDate,Description")] Activity activity)
        {
            if (id != activity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var activityOne = db.Activity.Find(id);
                try
                {
                    activityOne.Module = activity.Module;
                    activityOne.ModuleId = activityOne.ModuleId;
                    activityOne.Name = activity.Name;
                    activityOne.StartDate = activity.StartDate;
                    activityOne.EndDate = activity.EndDate;
                    activityOne.ActivityType = activity.ActivityType;
                    db.Update(activityOne);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"/courses?moduleID={activityOne.ModuleId}");
            }
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await db.Activity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await db.Activity.FindAsync(id);
            db.Activity.Remove(activity);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id)
        {
            return db.Activity.Any(e => e.Id == id);
        }


        private IEnumerable<SelectListItem> GetModulesSelectListItem()
        {
            var modules = db.Module.OrderBy(m=>m.Title);
            var GetModules = new List<SelectListItem>();
            foreach (var mod in modules)
            {
                var newType = (new SelectListItem
                {
                    Text = mod.Title,
                    Value = mod.Id.ToString(),
                });
                GetModules.Add(newType);
            }
            return (GetModules);
        }
    }
}
