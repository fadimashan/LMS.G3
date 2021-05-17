using LMS.Data.Data;
using LMS.Data.Repositories;
using LMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace LMS.Test3.Controller
{
    [TestClass]
    public class ModuleControllerTests1
    {

        // Mocking the dependency

        private readonly Mock<LMSWebContext> _mockRepo;
        private Mock<IUoW> mockUoW;
        private readonly ModulesController _controller;



        // Create a mock object.
        public ModuleControllerTests1()
        {
            _mockRepo = new Mock<LMSWebContext>();

            mockUoW = new Mock<IUoW>();
            var mockRepo = new Mock<IModulesRepository>();

            mockUoW.Setup(u => u.ModulesRepo).Returns(mockRepo.Object);

            _controller = new ModulesController(mockUoW.Object, _mockRepo.Object);

        }
        [Fact]
        public void Index_ActionExecutes_ReturnsViewForIndex()
        {
            var result = _controller.Index();

            Assert.IsType<ViewResult>(result);

            //[Fact]
            //public void Index_NotAutenthicated_ReturnsExpected()
            //{

            //    _controller.SetUserIsAuthenticated(false);               // Fake Http context



            //    var viewResult = (StatusCodeResult)_controller.Index().Result;

            //   // var actual = (Index)viewResult.Model;
            //   //Assert.Are
            //   // Assert.(StatusCodes.Status400BadRequest, viewResult.StatusCode);
            //}
            ////[TestMethod]
            ////public async Task Index_ReturnsViewResult_ShouldPass()
            ////{
            ////    _controller.SetUserIsAuthenticated(true);
            ////    var vm = new Index { ShowHistory = false };

            ////    var actual = await _controller.Index(vm);

            ////    Assert.IsInstanceOfType(actual, typeof(ViewResult));
            ////}
            ////[TestMethod]
            ////public void Create_ReturnsDefaultView_ShouldReturnNull()  // 
            ////{
            ////    _controller.SetAjaxRequest(false);
            ////    var actual = _controller.Create() as ViewResult;

            ////    Assert.IsNull(actual.ViewName);

            ////}
            ////[TestMethod]
            ////public void Create_ReturnsPartialViewWhenAjax_ShouldNotBeNull()
            ////{
            ////    _controller.SetAjaxRequest(true);
            ////    var actual = _controller.Create() as PartialViewResult;

            ////    Assert.IsNotNull(actual);
            ////}


            //// Private method for a List of Modules.

            //private List<Module> GetModuleList()
            //{
            //    return new List<Module>
            //    {
            //        new Module
            //        {
            //            Id = 1,
            //            Title = "C# Basics",
            //            Description = "Easy",
            //            StartDate = DateTime.Now.AddDays(3),
            //            EndDate = DateTime.Now.AddDays(1),

            //        },
            //        new Module
            //        {
            //            Id = 2,
            //            Title = "C# Advanced",
            //            Description = "Hard",
            //            StartDate = DateTime.Now.AddDays(-3),
            //            EndDate = DateTime.Now.AddDays(1),

            //        }
            //    };

            //    //// Testing Index Action.
            //    //// Test whether the action in the Module Controller
            //    //// returns a result of type ViewResult.The green test will confirm that the type is of type ViewResult.
            //    //// Otherwise it is red

            //    //[Fact]
            //    //public void Index_ActionExecutes_ReturnsViewForIndex()
            //    //{
            //    //    var result = _controller.Index();

            //    //    Assert.IsType<ViewResult>(result);
            //    //}

            //    //// Test to verify that Index action returns an exact number of modules
            //    //[Fact]
            //    //public void Index_ActionExecutes_ReturnsExactNumberOfModules()
            //    //{
            //    //    _mockRepo.Setup(_mockRepo => _mockRepo.GetAll())
            //    //        .Returns(new List<Module>() { new Module(), new Module() });

            //    //    //Act
            //    //    var result = _controller.Index();

            //    //    //Assert
            //    //    var viewResult = Assert.IsType<ViewResult>(result);
            //    //    var module = Assert.IsType<List<Module>>(viewResult.Model);
            //    //    Assert.Equal(2, module.Count);

            //    //    //Testing Create Actions.
            //    //    //There are two Create actions in the ModulesController class,the GET and POST action.
            //    //    //The first loads the Create view, which we will test below.

            //    //}
            //    //[Fact]
            //    //public void Create_ActionExecutes_ReturnsViewForCreate()
            //    //{
            //    //    var result = _controller.Create();

            //    //    Assert.IsType<ViewResult>(result);

            //    //    // The second POST action we first add a model error to ModelState property in order to test an invalid model state.
            //    //    // Then create a new module without the Title which makes it invalid as well.
            //    //    // Finally we call the create action and execute assertion which we verify the result is of type ViewResult and that the
            //    //    // model is of the module type. Additionally we are making sure that we get the same module back by comparing property
            //    //    // values from the testModule and the employee object.
            //    //}
            //    //[Fact]
            //    //public void Create_InvalidModelState_ReturnsView()
            //    //{
            //    //    _controller.ModelState.AddModelError("Role", "Role is required");

            //    //    var module = new Module {};

            //    //    //Act
            //    //    var result = _controller.Create(module);

            //    //    //Assert
            //    //    var viewResult = Assert.IsType<ViewResult>(result);
            //    //    var testModule = Assert.IsType<Module>(viewResult.Model);



            //    //}
            //}
        }
    }
}