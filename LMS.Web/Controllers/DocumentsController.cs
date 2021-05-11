using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities.ViewModels;
using LMS.Core.Entities;
using LMS.Data.Data;

namespace LMS.Web.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly MvcDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentsController(MvcDbContext context, UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        //GET: Documents
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Document
                .OrderBy(d => d.Name)
                .ToListAsync());
        }
        
        //GET: Read exist Files
        public IActionResult Files(int? courseId , int? moduleId, int? activityId)
        {
            // Get files from the server
            var model = new FilesViewModel();
            var userId = _userManager.GetUserId(User);

            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files")))
            {

                model.Files.Add(
                    new FileDetails 
                    {
                        Name = System.IO.Path.GetFileName(item),
                        UserName = User.Identity.Name,
                        UploadTime = DateTime.Now,
                        CourseId = courseId,
                        ModuleId = moduleId,
                        ActivityId = activityId,
                        UserId = userId,
                        Path = item 
                    }) ;
            }
            return View(model);
        }

        /// <summary>
        /// Uploaded files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Files(IFormFile[] files)
        {
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
                }
                ViewBag.Message = "Files are successfully uploaded";
            }

            var model = new FilesViewModel();
            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files")))
            {
                model.Files.Add(
                    new FileDetails
                    {
                        Name = System.IO.Path.GetFileName(item), 
                        Path = item
                    });
            }
            return View(model);
        }
        
        /// <summary>
        /// Downlaod Files
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<IActionResult> Download(string filename)
        {
            if (filename is null)
            {
                return Content("filename is not availble");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filename);

            var memory = new MemoryStream();
            
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var document = await _dbContext.Document
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (document is null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UploadTime")] Document document)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.AddAsync(document);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var document = await _dbContext.Document.FindAsync(id);
            
            if (document is null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UploadTime")] Document document)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(document);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
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
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var document = await _dbContext.Document
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (document is null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _dbContext.Document.FindAsync(id);
            _dbContext.Document.Remove(document);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _dbContext.Document.Any(e => e.Id == id);
        }
    }
}
