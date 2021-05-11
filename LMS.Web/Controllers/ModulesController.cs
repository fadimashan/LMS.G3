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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;
using LMS.Core.Entities.ViewModels;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class ModulesController : Controller
    {
        private readonly LMSWebContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ModulesController(LMSWebContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
           _userManager = userManager;
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

            var @module = await db.Module.Include(m=> m.Documents).Include(m=> m.Activities)
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
                db.Add(activity);
                await db.SaveChangesAsync();
                return Redirect($"/modules/details/{activity.ModuleId}");

            }
            return Redirect("/courses");
        }

        [HttpPost]
        public IActionResult UploadModuleDocument(int id, IFormFile[] files)
        {
            var mod = db.Module.Find(id);
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
                        ModuleId = id,
                        UploadTime = DateTime.Now,
                        Description = filePath,
                        UserId = userId
                    };
                    db.Document.Add(doc);

                }
                db.SaveChanges();
            }

            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files")))
            {

                var model = new FilesViewModel();
                model.Files.Add(
                    new FileDetails
                    {
                        Name = System.IO.Path.GetFileName(item),
                        UserName = User.Identity.Name,
                        UploadTime = DateTime.Now,
                        ModuleId = id,
                        UserId = userId,
                        Path = item
                    });
            }

            //return View(model);
            return Redirect($"/Modules/details/{mod.Id}");

        }

    }
}
