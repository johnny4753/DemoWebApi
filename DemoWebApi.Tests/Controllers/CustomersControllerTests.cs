using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using DemoWebApi.Interface;
using DemoWebApi.Models.Domain;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;

namespace DemoWebApi.Controllers.Tests
{
    [TestClass()]
    public class CustomersControllerTests
    {
        [TestMethod()]
        public void GetReturnsCustomerWithSameId()
        {
            // Arrange
            var customer = Builder<Customer>.CreateNew()  //[NBuilder]
                            .With(x => x.CustomerID = "testId")
                            .Build();
            var customers = new List<Customer>{customer};
            var mockRepository = new Mock<IRepository<Customer>>(); //[Moq]
            mockRepository.Setup(x => x.GetAll())
                .Returns(customers.AsQueryable());

            var target = new CustomersController(mockRepository.Object);

            // Act
            IHttpActionResult actionResult = target.Get("testId");
            var contentResult = actionResult as OkNegotiatedContentResult<Customer>;

            // Assert
            contentResult.Should().NotBeNull();
            contentResult.Content.CustomerID.Should().Be("testId");
        }

        [TestMethod]
        public void GetReturnsNotFound()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Customer>>();
            var target = new CustomersController(mockRepository.Object);

            // Act
            IHttpActionResult actionResult = target.Get("notExistId");

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }
    }
}