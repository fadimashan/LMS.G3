using System.Collections.Generic;
using AutoMapper;
using LMS.API.Controllers;
using LMS.API.Models.Entities;
using LMS.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

//namespace LMS.Tests.Controller
//{
//    /// <summary>
//    /// Test to return Authors by FirstName
//    /// </summary>
//    /// 

//    public class AuthorsControllerTests
//    {
//        private readonly AuthorsController authorController;
//        private readonly Mock<List<Author>> mockAuthorsList;
//       
        

//        public AuthorsControllerTests()
//        {
//            // Ändra put-metoden i AuthorsController så den inte använder context (se Dimitris EventDay)
//            // Ta bort context från Authorscontroller
//            // Få in AutoMapper i testklassen och mocka den också

//            mockAuthorsList = new Mock<List<Author>>();
//            authorController = new AuthorsController(authorsRepository,mapper);
            
            
//        }

//        [Fact]
//        public void GetAuthorsTest_ReturnsListOfAuthorsByFirstName()
//        {
//            //arrange

//            var mockAuthors = new List<Author> {
//                new Author{FirstName = "Tdd One"},
//                new Author{FirstName = "Tdd Two"}
//            };

//            mockAuthorsList.Object.AddRange(mockAuthors);

//            // act

//            var result = authorController.GetAuthors();

//            //assert
//            var model = Assert.IsAssignableFrom<ActionResult<List<Author>>>(result);
//            Assert.Equal(2, model.Value.Count);

//        }

//    }

//    //internal AuthorsResponse

//    ///// <summary>
//    ///// Testing the Index(), action. Two scenarios can be tested.
//    ///// 1. Modules are not returned
//    ///// 2. Modules are returned.
//    ///// </summary>
//    //public class ModulesControllerTests
//    //{
        
        

//    //}
//}

