using System.Collections.Generic;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;

namespace LMS.Tests.Controllers
{
    [TestClass]
    public class LmsApiClassesTests
    {
        [SetUp]
        private readonly AuthorsController _authorController;
        private Mock<List<Author>> _mockAuthorsList;
        private readonly IMapper mapper;



        public AuthorRequestTest()
        {
            _mockAuthorsList = new Mock<List<Author>>();
            // Ändra put-metoden i AuthorsController så den inte använder context (se Dimitris EventDay)
            // Ta bort context från Authorscontroller
            // Få in AutoMapper i testklassen och mocka den också
            _authorController = new AuthorsController(_mockAuthorsList.Object);
            _mapper = new IMapper();
        }

        [Fact]
        public void GetTest_ReturnsListOfAuthors()
        {
            //arrange

            var mockAuthors = new List<Author> {
                new Author{FirstName = "Tdd One"},
                new Author{FirstName = "Tdd Two"}

}        var result = _blogController.Get();

            //assert
            var model = Assert.IsAssignableFrom<ActionResult<List<Post>>>(result);
            Assert.Equal(2, model.Value.Count);

        }
    }
