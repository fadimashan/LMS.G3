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
using System.Linq;

namespace LMS.Web.Controllers
{
    public class AuthorsController : Controller
    {
        private const string baseAddress = "https://localhost:5001/";

        private readonly ILogger<AuthorsController> _logger;
        private readonly HttpClient httpClient;

        public AuthorsController(ILogger<AuthorsController> logger)
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


        public async Task<IActionResult> Index(string name)
        {
            
            IEnumerable<AuthorDto> authors;
            var response = await httpClient.GetAsync("api/authors");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(name))
            {
                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDto>>(content);
                    authors = authors.Where(a => a.FirstName.ToLower().StartsWith(name.ToLower()) || a.LastName.ToLower().StartsWith(name.ToLower()));
                }
                else
                {
                    var xmlSerializer = new XmlSerializer(typeof(AuthorDto));
                    authors = (IEnumerable<AuthorDto>)xmlSerializer.Deserialize(new StringReader(content));
                    authors = authors.Where(a => a.FirstName.ToLower().StartsWith(name.ToLower()) || a.LastName.ToLower().StartsWith(name.ToLower())
                    );
                }
            }
            else
            {

                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDto>>(content);

                }
                else
                {
                    var xmlSerializer = new XmlSerializer(typeof(AuthorDto));
                    authors = (IEnumerable<AuthorDto>)xmlSerializer.Deserialize(new StringReader(content));


                }  
            }
            return View(authors);
        }




        public async Task<IActionResult> GetAuthor(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            AuthorDto author;

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                author = JsonConvert.DeserializeObject<AuthorDto>(content);
            }
            else
            {
                var xmlSerializer = new XmlSerializer(typeof(AuthorDto));
                author = (AuthorDto)xmlSerializer.Deserialize(new StringReader(content));
            }
            return View(author);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,DateOfBirth")] AuthorCreationDto author)
        {
            var jsonData = JsonConvert.SerializeObject(author);
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/authors")
            {
                Content = new StringContent(jsonData)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content1 = await response.Content.ReadAsStringAsync();
            var newAuthor1 = JsonConvert.DeserializeObject<AuthorDto>(content1);



            var fullName = author.FirstName + author.LastName;
            var requestByName = new HttpRequestMessage(HttpMethod.Get, $"api/authors/GetAuthorByName/{fullName}");
            requestByName.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var responseByName =  await httpClient.SendAsync(requestByName);

            responseByName.EnsureSuccessStatusCode();
            if (responseByName.IsSuccessStatusCode == false)
            {
                return NotFound();
            }
            var content = await responseByName.Content.ReadAsStringAsync();

            var newAuthor = JsonConvert.DeserializeObject<AuthorDto>(content);

            return Redirect($"Details/{newAuthor.Id}");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/Edit/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode == false)
            {
                return NotFound();
            }
            var content = await response.Content.ReadAsStringAsync();
            AuthorCreationDto newAuthor;

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                newAuthor = JsonConvert.DeserializeObject<AuthorCreationDto>(content);
            }
            else
            {
                var xmlSerializer = new XmlSerializer(typeof(AuthorCreationDto));
                newAuthor = (AuthorCreationDto)xmlSerializer.Deserialize(new StringReader(content));
            }

            return View(newAuthor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,DateOfBirth")] AuthorCreationDto author)
        {
            var jsonData = JsonConvert.SerializeObject(author);
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/authors/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(jsonData);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode == false)
            {
                return NotFound();
            }
            var content = await response.Content.ReadAsStringAsync();
            AuthorWithPublicationsDto author;

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                author = JsonConvert.DeserializeObject<AuthorWithPublicationsDto>(content);
            }
            else
            {
                var xmlSerializer = new XmlSerializer(typeof(AuthorWithPublicationsDto));
                author = (AuthorWithPublicationsDto)xmlSerializer.Deserialize(new StringReader(content));
            }

            return View(author);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            AuthorDto author;

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                author = JsonConvert.DeserializeObject<AuthorDto>(content);
            }
            else
            {
                var xmlSerializer = new XmlSerializer(typeof(AuthorDto));
                author = (AuthorDto)xmlSerializer.Deserialize(new StringReader(content));
            }
            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/authors/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index");
        }
    }
}
