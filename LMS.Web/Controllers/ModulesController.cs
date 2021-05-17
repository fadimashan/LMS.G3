using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;
using LMS.Data.Data;

using System.IO;
using LMS.Core.Entities.ViewModels;

namespace LMS.Web.Controllers
{
    [Authorize]
    public class ModulesController : Controller
    {
        private readonly MvcDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ModulesController(MvcDbContext context, UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
           _userManager = userManager;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Module.ToListAsync());
        }

        // GET: Modules/Details/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!_dbContext.Module.Any(m => m.Id == id))
            {
                return NotFound();
            }

            var module = await _dbContext.Module
                .Include(m=> m.Documents)
                .Include(m=> m.Activities)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (module is null)
            {
                return NotFound();
            }

            return View(module);
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
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartDate,EndDate,CourseId")] Module module)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.AddAsync(module);
                await _dbContext.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));
                return Redirect("/courses");
            }
            //return View(module);
            return Redirect("/courses");
        }

        // GET: Modules/Edit/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var module = await _dbContext.Module.FindAsync(id);
            
            if (module is null)
            {
                return NotFound();
            }
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,StartDate,EndDate")] Module module)
        {
            var moduleFromContext = _dbContext.Module.Find(id);

            if (moduleFromContext is null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    moduleFromContext.Title = module.Title;
                    moduleFromContext.StartDate = module.StartDate;
                    moduleFromContext.EndDate = module.EndDate;
                    moduleFromContext.Description = module.Description;
                    _dbContext.Update(moduleFromContext);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(module.Id))
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
            return View(module);
        }

        // GET: Modules/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var module = await _dbContext.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (module is null)
            {
                return NotFound();
            }

            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var module = await _dbContext.Module.FindAsync(id);
            _dbContext.Module.Remove(module);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return _dbContext.Module.Any(e => e.Id == id);
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

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateFromModule(int id)
        {
            var module = await _dbContext.Module.FindAsync(id);
            
            if (module is null)
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
        public async Task<IActionResult> CreateFromModule(int id, [Bind("Name,ActivityType,StartDate,EndDate,Description,ModuleId")] Activity activity)
        {
            // Variable `moduleFromContext` is never used
            var moduleFromContext = _dbContext.Module.Find(id);
            if (ModelState.IsValid)
            {
                await _dbContext.AddAsync(activity);
                await _dbContext.SaveChangesAsync();
                return Redirect($"/modules/details/{activity.ModuleId}");
            }
            return Redirect("/courses");
        }

        public async Task<IActionResult> ModuleFiles(int id)
        {
            
            var firstModuleID = await _dbContext.Module.Where(m => m.Id == id)
                    .Include(c => c.Documents)
                    .FirstOrDefaultAsync();

            return View("ModuleFiles", firstModuleID);

        }

        [HttpPost]
        public IActionResult UploadModuleDocument(int id, IFormFile[] files)
        {
            var moduleFromContext = _dbContext.Module.Find(id);
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
                        ModuleId = id,
                        UploadTime = DateTime.Now,
                        Description = filePath,
                        UserId = userId
                    };
                    _dbContext.Document.Add(doc);
                }
                _dbContext.SaveChanges();
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

            if (User.IsInRole("Student"))
            {
                return RedirectToAction("ModuleFiles", moduleFromContext);

            }
            //return View();
            return Redirect($"/Modules/details/{moduleFromContext.Id}");
        }


        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyModuleStartDate(Module module)
        {
        
            var course = _dbContext.Course.Find(module.CourseId);

            if (module.EndDate != DateTime.Parse("0001-01-01 00:00:00") && (module.StartDate < course.StartDate || module.EndDate > course.EndDate || module.StartDate > course.EndDate || module.EndDate < course.StartDate))
            {
                return Json($"Course started in {course.StartDate}. End in { course.EndDate} ");
            }

            if (module.EndDate != DateTime.Parse("0001-01-01 00:00:00") && (module.StartDate >= module.EndDate))
            {
                return Json($"Date should not start and end in the same time!");

            }

            return Json(true);
        }


    }
}
