using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LMS.Web.Models;
using LMS.Core.Models;

namespace LMS.Web.Controllers
{
    public class HomeController : Controller
    {
        // private readonly ILogger<HomeController> _logger;
        // private readonly HttpClient httpClient;
        
        /*
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5001/");
            httpClient.Timeout = new TimeSpan(0, 0, 10);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        */
        
        public IActionResult Index()
        {
            return View();
        }
        
        /*
        public async Task<IActionResult> GetAuthors()
        {
            var response = await httpClient.GetAsync("api/authors");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            IEnumerable<AuthorDto> authors;
            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDto>>(content);
            }
            else {
                var xmlSerialiser = new XmlSerializer(typeof(AuthorDto));
                authors = (IEnumerable<AuthorDto>)xmlSerialiser.Deserialize(new StringReader(content));
            }
            return View(authors);
        }

        public async Task<IActionResult> GetAuthor(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            AuthorDto author;

            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                author = JsonConvert.DeserializeObject<AuthorDto>(content);
            }
            else
            {
                var xmlSerialiser = new XmlSerializer(typeof(AuthorDto));
                author = (AuthorDto)xmlSerialiser.Deserialize(new StringReader(content));
            }
            return View(author);
        }
        */

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
