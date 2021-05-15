using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LMS.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace LMS.Web.Controllers
{
    public class LiteratureController : Controller
    {
        private const string baseAddress = "https://localhost:5001/";
        private const string baseRoute = "api/publications";
        
        private readonly ILogger<LiteratureController> _logger;
        private readonly HttpClient httpClient;

        public LiteratureController(ILogger<LiteratureController> logger)
        {
            _logger = logger;

            // FIXME: This is a temporary solution. Fix SSL certificate to prevent crashes
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseAddress), 
                Timeout = new TimeSpan(0, 0, 10)
            };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET
        public async Task<IActionResult> Index(string searchQuery)
        {
            var uri = baseRoute + (string.IsNullOrWhiteSpace(searchQuery) ? "" : $"?searchQuery={searchQuery}");
            var response = await httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            IEnumerable<PublicationWithAuthorsDto> publications;
            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                publications = JsonConvert.DeserializeObject<IEnumerable<PublicationWithAuthorsDto>>(content);
            }
            else
            {
                var xmlSerializer = new XmlSerializer(typeof(PublicationWithAuthorsDto));
                publications = (IEnumerable<PublicationWithAuthorsDto>)xmlSerializer.Deserialize(new StringReader(content));
            }
            return View(publications);
            
/*
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            IEnumerable<PublicationWithAuthorsDto> publications;
            if(!string.IsNullOrEmpty(searchQuery))
            {
                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    publications = JsonConvert.DeserializeObject<IEnumerable<PublicationWithAuthorsDto>>(content);
                    publications = publications.Where(p => p.Title.ToLower().StartsWith(searchQuery) || p.Title.ToUpper().StartsWith(searchQuery));
                }
                else
                {
                    var xmlSerializer = new XmlSerializer(typeof(PublicationWithAuthorsDto));
                    publications = (IEnumerable<PublicationWithAuthorsDto>)xmlSerializer.Deserialize(new StringReader(content));
                    publications = publications.Where(p => p.Title.ToLower().StartsWith(searchQuery) || p.Title.ToUpper().StartsWith(searchQuery));
                }
            }
            else
            {
                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    publications = JsonConvert.DeserializeObject<IEnumerable<PublicationWithAuthorsDto>>(content);
                }
                else
                {
                    var xmlSerializer = new XmlSerializer(typeof(PublicationWithAuthorsDto));
                    publications = (IEnumerable<PublicationWithAuthorsDto>)xmlSerializer.Deserialize(new StringReader(content));
                }
            }
            return View(publications);
            */
        }

        // GET: Publications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            // var request = new HttpRequestMessage(HttpMethod.Get, $"api/publications/{id}");
            var request = new HttpRequestMessage(HttpMethod.Get, string.Join("/", baseRoute, id.ToString()));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode == false)
            {
                return NotFound();
            }
            
            var content = await response.Content.ReadAsStringAsync();

            PublicationWithAuthorsDto publication;

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                publication = JsonConvert.DeserializeObject<PublicationWithAuthorsDto>(content);
            }
            else
            {
                var xmlSerializer = new XmlSerializer(typeof(PublicationWithAuthorsDto));
                publication = (PublicationWithAuthorsDto)xmlSerializer.Deserialize(new StringReader(content));
            }

            return View(publication);
        }

        // GET: Publications/Create
        public IActionResult Create()
        {
            var items = GetAuthors().Result;
            var multiItems = new MultiSelectList(items.OrderBy(i=> i.Text), "Value","Text");
            var model = new PublicationCreationDto()
            {
                GetSubjects = GetSubjects().Result,
                GetAuthors = multiItems,
                GetTypes = GetTypes().Result

            };
            return View(model);
        }

        // POST: Publications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title, Description, PublicationDate, Level, TypeId, SubjectId, AuthorIds")] PublicationCreationDto publication)
        {
            var jsonData = JsonConvert.SerializeObject(publication);

            var request = new HttpRequestMessage(HttpMethod.Post, baseRoute)
            {
                Content = new StringContent(jsonData)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // var content = await response.Content.ReadAsStringAsync();
            // var createdPublication = JsonConvert.DeserializeObject<PublicationWithAuthorsDto>(content);
            // return RedirectToAction($"Details/{createdPublication.Id}");
            return RedirectToAction(nameof(Index));
            // return View("Details",createdPublication);
        }

        // GET: Publications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, string.Join("/", baseRoute, id.ToString()));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode == false)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();

            var publication = JsonConvert.DeserializeObject<PublicationWithAuthorsDto>(content);

            return View(publication);
        }

        // POST: Publications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title, Description, PublicationDate, Level, TypeId, SubjectId")] PublicationCreationDto publication)
        {
            var jsonData = JsonConvert.SerializeObject(publication);

            var request = new HttpRequestMessage(HttpMethod.Put, string.Join("/", baseRoute, id.ToString()))
            {
                Content = new StringContent(jsonData)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // var content = await response.Content.ReadAsStringAsync();
            // var updatedPublication = JsonConvert.DeserializeObject<PublicationWithAuthorsDto>(content);
            // return View();
            return RedirectToAction(nameof(Index));
        }

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, string.Join("/", baseRoute, id.ToString()));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();

            PublicationWithAuthorsDto publication = JsonConvert.DeserializeObject<PublicationWithAuthorsDto>(content);
            
            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, string.Join("/", baseRoute, id.ToString()));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        /*
        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage,
            X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
        {
            return sslErrors == SslPolicyErrors.None;
        }
        */
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                httpClient?.Dispose();
            }

            base.Dispose(disposing);
        }


        public async Task<IEnumerable<SelectListItem>> GetSubjects()
        {
            var subjects = new List<SelectListItem>();

            var request = new HttpRequestMessage(HttpMethod.Get, "api/publications/subjects");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
          

            var content = await response.Content.ReadAsStringAsync();

            var subjectList = JsonConvert.DeserializeObject<IEnumerable<SubjectDto>>(content);

            foreach (var subject in subjectList)
            {
                var selectListItem = (new SelectListItem
                {
                    Text = subject.Name,
                    Value = subject.Id.ToString()
                });
                subjects.Add(selectListItem);
            }
            return (subjects);
        }

        public async Task<IEnumerable<SelectListItem>> GetAuthors()
        {
            var selectList = new List<SelectListItem>();

            var response = await httpClient.GetAsync("api/authors");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            IEnumerable<AuthorDto> authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDto>>(content);
          
            foreach (var author in authors)
            {
                var authorName = author.FirstName + " " + author.LastName;
                var selectListItem = new SelectListItem()
                {
                    
                    Text = authorName,
                    Value = author.Id.ToString()
                };
                selectList.Add(selectListItem);
            }
            return (selectList);
        }
        
        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var selectList = new List<SelectListItem>();

            var response = await httpClient.GetAsync("api/publications/types");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            IEnumerable<PublicationTypeDto> types = JsonConvert.DeserializeObject<IEnumerable<PublicationTypeDto>>(content);
          
            foreach (var type in types)
            {
                var selectListItem = new SelectListItem()
                {
                    
                    Text = type.Name,
                    Value = type.Id.ToString()
                };
                selectList.Add(selectListItem);
            }
            return (selectList);
        }
    }
}
