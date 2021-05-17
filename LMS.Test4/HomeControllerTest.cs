using System;
using LMS.Web.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LMS.Test4
{
    public class HomeControllerTest
    {

        private readonly HomeController _sut;
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        public HomeControllerTest()
        {
            _sut = new HomeController((ILogger<HomeController>)_loggerMock.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnHomeViewPage()
        {


            //Arrange


            //Act


            //Assert
        }
    }
}
