using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.Tests.ControllerTests
{
    public class CategoryControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;


        public CategoryControllerTests()
        {
            //Dependencies
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();

        }

        [Fact]
        public void CategoryController_Index_ReturnsViewResultWithCategoryList()
        {
            // Arrange
            
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Drama", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Horror", DisplayOrder = 2 }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<string?>())).Returns(categories);
            _unitOfWorkMock.Setup(uow => uow.Category).Returns(_categoryRepositoryMock.Object);

            var controller = new CategoryController(_unitOfWorkMock.Object); // SUT (System Under Test) created here

            // Act
            
            var result = controller.Index();

            // Assert

            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.Model.Should().BeEquivalentTo(categories);

        }
    }
}
