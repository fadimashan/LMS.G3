using LMS.Data.Data;
using LMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;

//namespace LMS.Test2.Controller
//{
//    [TestClass]
//    public class ModuleClassesTests
//    {
//        // Mocking the dependency

//        private readonly Mock<LMSWebContext> _mockRepo;
//        private readonly ModulesController _controller;

//        // Create an mock object.
//        public ModuleClassesTests()
//        {
//            _mockRepo = new Mock<LMSWebContext>();
//            _controller = new ModulesController(_mockRepo.Object);
//        }

//        // Testing Index action. Test whether it fetches all modules from database and returns a view with the modules.
           
//        [Fact]
//        public void Index_ActionExecutes_ReturnsViewForIndex()
//        {
//            var result = _controller.Create();
        
//            Assert.IsTrue<ViewResult>(result);
//        }

//    //    [Fact]
//    //    public void Index_ActionExecutes_ReturnsExactNumberOfModules()
//    //    {
//    //        _mockRepo.Setup(repo => repo.GetAll())
//    //            .Returns(new List<Module>() { new Module(), new Module() });

//    //        var result = _controller.Index();

//    //        var viewResult = Assert.IsType<ViewResult>(result);
//    //        var modules = Assert.IsType<List<Module>>(viewResult.Model);
//    //        Assert.Equal(2, modules.Count);
//    //    }

//    //    [Fact]
//    //    public void Create_ActionExecutes_ReturnsViewForCreate()
//    //    {
//    //        var result = _controller.Create();

//    //        Assert.IsType<ViewResult>(result);
//    //    }

//    //    [Fact]
//    //    public void Create_InvalidModelState_ReturnsView()
//    //    {
//    //        _controller.ModelState.AddModelError("Name", "Name is required");

//    //        var module = new Module { Age = 25, AccountNumber = "255-8547963214-41" };

//    //        var result = _controller.Create(module);

//    //        var viewResult = Assert.IsType<ViewResult>(result);
//    //        var testModule = Assert.IsType<Module>(viewResult.Model);
//    //        Assert.Equal(module.AccountNumber, testmodule.AccountNumber);
//    //        Assert.Equal(employee.Age, testEmployee.Age);
//    //    }

//    //    [Fact]
//    //    public void Create_InvalidModelState_CreateEmployeeNeverExecutes()
//    //    {
//    //        _controller.ModelState.AddModelError("Name", "Name is required");

//    //        var employee = new Employee { Age = 34 };

//    //        _controller.Create(employee);

//    //        _mockRepo.Verify(x => x.CreateEmployee(It.IsAny<Employee>()), Times.Never);
//    //    }

//    //    [Fact]
//    //    public void Create_ModelStateValid_CreateEmployeeCalledOnce()
//    //    {
//    //        Employee emp = null;
//    //        _mockRepo.Setup(r => r.CreateEmployee(It.IsAny<Employee>()))
//    //            .Callback<Employee>(x => emp = x);

//    //        var employee = new Employee
//    //        {
//    //            Name = "Test Employee",
//    //            Age = 32,
//    //            AccountNumber = "123-5435789603-21"
//    //        };

//    //        _controller.Create(employee);

//    //        _mockRepo.Verify(x => x.CreateEmployee(It.IsAny<Employee>()), Times.Once);

//    //        Assert.Equal(emp.Name, employee.Name);
//    //        Assert.Equal(emp.Age, employee.Age);
//    //        Assert.Equal(emp.AccountNumber, employee.AccountNumber);
//    //    }

//    //    [Fact]
//    //    public void Create_ActionExecuted_RedirectsToIndexAction()
//    //    {
//    //        var employee = new Employee
//    //        {
//    //            Name = "Test Employee",
//    //            Age = 45,
//    //            AccountNumber = "123-4356874310-43"
//    //        };

//    //        var result = _controller.Create(employee);

//    //        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
//    //        Assert.Equal("Index", redirectToActionResult.ActionName);


//    //        //}
//    //        //[TestInitialize]
//    //        //public void SetUp()
//    //        //{


//    //        ////Arrange
//    //        //var mockLogger = new Mock<ILogger<HomeController>>();
//    //        //var controller = new HomeController(mockLogger.Object);

//    //        //// Act
//    //        //var results = controller.Index();

//    //        //// Assert

//    //        //Assert.Equals()


//    //        ////var mockContext = new Mock<LMSWebContext>();
//    //        ////var controller = new ModulesController();
//    //        ////var documentController = new DocumentsController();
//    //        ////var coursesController = new CoursesController();
//    //        ////var activitiesControler = new ActivitiesController();

//    //    }
//    }
//}
