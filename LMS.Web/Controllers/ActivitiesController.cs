using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;
using LMS.Data.Data;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly MvcDbContext _dbContext;

        public ActivitiesController(MvcDbContext context)
        {
            _dbContext = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Activity.ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var activity = await _dbContext.Activity.FirstOrDefaultAsync(m => m.Id == id);

            if (activity is null)
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
                _dbContext.Add(activity);
                await _dbContext.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return Redirect("/courses");
            }
            //return View(activity);
            return Redirect("/courses");
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var activity = await _dbContext.Activity.FindAsync(id);

            if (activity is null)
            {
                return NotFound();
            }

            activity.GetModulesSelectListItem = GetModulesSelectListItem();
            
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
                var activityOne = _dbContext.Activity.Find(id);
                try
                {
                    activityOne.Module = activity.Module;
                    activityOne.ModuleId = activityOne.ModuleId;
                    activityOne.Name = activity.Name;
                    activityOne.StartDate = activity.StartDate;
                    activityOne.EndDate = activity.EndDate;
                    activityOne.ActivityType = activity.ActivityType;
                    _dbContext.Update(activityOne);
                    await _dbContext.SaveChangesAsync();
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
                return Redirect($"/modules/details/{activityOne.ModuleId}");
            }
            //return View(activity);
            return Redirect("/courses");
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var activity = await _dbContext.Activity.FirstOrDefaultAsync(m => m.Id == id);

            if (activity is null)
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
            var activity = await _dbContext.Activity.FindAsync(id);
            _dbContext.Activity.Remove(activity);
            await _dbContext.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return Redirect("/courses");
        }

        private bool ActivityExists(int id)
        {
            return _dbContext.Activity.Any(e => e.Id == id);
        }
        
        private IEnumerable<SelectListItem> GetModulesSelectListItem()
        {
            var modules = _dbContext.Module.OrderBy(m=>m.Title);
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
