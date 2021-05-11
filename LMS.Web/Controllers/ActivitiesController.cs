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
using LMS.Core.Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly LMSWebContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public ActivitiesController(LMSWebContext context, UserManager<ApplicationUser> UserManager)
        {
            db = context;
            userManager = UserManager;
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

            var activity = await db.Activity.Include(a => a.Documents)
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
                //return RedirectToAction(nameof(Index));
                return Redirect("/courses");

            }
            //return View(activity);
            return Redirect("/courses");
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
                return Redirect($"/modules/details/{activityOne.ModuleId}");

            }
            //return View(activity);
            return Redirect("/courses");

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
            //return RedirectToAction(nameof(Index));
            return Redirect("/courses");

        }

        private bool ActivityExists(int id)
        {
            return db.Activity.Any(e => e.Id == id);
        }


        private IEnumerable<SelectListItem> GetModulesSelectListItem()
        {
            var modules = db.Module.OrderBy(m => m.Title);
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
            var activity = db.Activity.Find(id);
            var userId = userManager.GetUserId(User);
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
                        ActivityId = id,
                        UploadTime = DateTime.Now,
                        Description = filePath,
                        UserId = userId
                    };
                    db.Document.Add(doc);

                }
                db.SaveChanges();
                ViewBag.Message = "Files are successfully uploaded";
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
                        ActivityId = id,
                        UserId = userId,
                        Path = item
                    });
            }

            return Redirect($"/Activities/Details/{activity.Id}");

        }
    }
}
