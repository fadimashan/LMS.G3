using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;
using LMS.Core.Entities.ViewModels;
using LMS.Data.Data;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly MvcDbContext _dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public ActivitiesController(MvcDbContext context, UserManager<ApplicationUser> UserManager)
        {
            _dbContext = context;
            userManager = UserManager;
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

            var activity = await _dbContext.Activity
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(m => m.Id == id);
            
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
                await _dbContext.AddAsync(activity);
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
            var module = await _dbContext.Module.FindAsync(activity.ModuleId);

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ActivityType,StartDate,EndDate,Description,ModuleId,Module")] Activity activity)
        {
            if (id != activity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var activityFromContext = _dbContext.Activity.Find(id);
                try
                {
                    activityFromContext.Module = activity.Module;
                    activityFromContext.ModuleId = activityFromContext.ModuleId;
                    activityFromContext.Name = activity.Name;
                    activityFromContext.StartDate = activity.StartDate;
                    activityFromContext.EndDate = activity.EndDate;
                    activityFromContext.ActivityType = activity.ActivityType;
                    _dbContext.Update(activityFromContext);
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
                return Redirect($"/modules/details/{activityFromContext.ModuleId}");
            }
            //return View(activity);
            return Redirect("/courses");
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyStartDate(Activity activity)
        {
            var module = _dbContext.Module.Find(activity.ModuleId);

            if (activity.EndDate != DateTime.Parse("0001-01-01 00:00:00") && (activity.StartDate < module.StartDate || activity.EndDate > module.EndDate || activity.StartDate > module.EndDate || activity.EndDate < module.StartDate))
            {
                return Json($"Module started in {module.StartDate}. End in { module.EndDate} ");
            }

            if(activity.EndDate != DateTime.Parse("0001-01-01 00:00:00") && (activity.StartDate >= activity.EndDate))
            {
                return Json($"Date should not start and end in the same time!");

            }

            return Json(true);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var activity = await _dbContext.Activity
                .FirstOrDefaultAsync(m => m.Id == id);
            
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
            var modules = _dbContext.Module.OrderBy(m => m.Title);
            var GetModules = new List<SelectListItem>();
            foreach (var module in modules)
            {
                var newType = (new SelectListItem
                {
                    Text = module.Title,
                    Value = module.Id.ToString(),
                });
                GetModules.Add(newType);
            }
            return (GetModules);
        }
        
        //// POST: Activities/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public IActionResult UploadActivity(int id)
        //{
        //    var model = new FilesViewModel();
        //    var userId = userManager.GetUserId(User);

        //    foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files")))
        //    {
        //        model.Files.Add(
        //            new FileDetails
        //            {
        //                Name = System.IO.Path.GetFileName(item),
        //                UserName = User.Identity.Name,
        //                UploadTime = DateTime.Now,
        //                ActivityId = id,
        //                UserId = userId,
        //                Path = item
        //            });
        //    }

        //    return View(model);
        //}

        [HttpPost]
        public IActionResult UploadActivity(int id, IFormFile[] files)
        {
            var activity = _dbContext.Activity.Find(id);
            var userId = userManager.GetUserId(User);
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
                        ActivityId = id,
                        UploadTime = DateTime.Now,
                        Description = filePath,
                        UserId = userId
                    };
                    _dbContext.Document.Add(doc);

                }
                _dbContext.SaveChanges();
                ViewBag.Message = "Files are successfully uploaded";
            }

            var model = new FilesViewModel();
            // TODO: The path string "wwwroot/files" should be moved to properties and retrieved from there when needed
            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files")))
            {
                model.Files.Add(
                    new FileDetails
                    {
                        Name = System.IO.Path.GetFileName(item),
                        UserName = User.Identity.Name,
                        UploadTime = DateTime.Now,
                        ActivityId = id,
                        UserId = userId,
                        Path = item
                    });
            }

            return Redirect($"/Activities/Details/{activity.Id}");
        }
    }
}
