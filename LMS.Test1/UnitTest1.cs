
using System;
using System.Threading.Tasks;
using AutoMapper;
using LMS.API.Controllers;
using LMS.API.Models.DTO;
using LMS.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

//namespace LMS.Test1
//{
//    [TestClass]
//    public class AuthorsControllerTests
//    {
//        // Mock
//        private readonly AuthorsController _authorsController;
//        private readonly Mock<IAuthorsRepository> _authorRepoMock = new Mock<IAuthorsRepository>();
//        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

//        public AuthorsControllerTests()
//        {
//            _authorsController = new AuthorsController(_authorRepoMock.Object,_mapper.Object);
//        }

//        [TestMethod]

//        // Get customer by Id
//        public async Task GetAuthor_ShouldReturnAuthor_WhenAuthorExists()
//        {
//            //Arrange
//            var authorId = Guid.NewGuid();
//            var authorFirstName = "TDD";
//            var authorDto = new AuthorDto
//            {
//                Id = authorId.ToString(),
//                FirstName = authorFirstName

//            };

//            _authorRepoMock.Setup(expression:x:IAuthorRepository => x.GetAuthor(authorId));



//            //Act

//            var author = await _authorsController.GetAuthor(authorId);

//            //Assert
//            Assert.Equals(expected: authorId, actual:author.id);
//        }
        
        
//    }
//}
