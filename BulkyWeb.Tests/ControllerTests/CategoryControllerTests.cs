using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        [Fact]
        public void CategoryController_Create_Get_ReturnsViewResult()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Category).Returns(_categoryRepositoryMock.Object);
            var controller = new CategoryController(_unitOfWorkMock.Object);

            // Act
            var result = controller.Create();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.Model.Should().BeNull();
        }

        [Fact]
        public void CategoryController_Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var newCategory = new Category { Id = 3, Name = "Comedy", DisplayOrder = 3 };
            _categoryRepositoryMock.Setup(repo => repo.Add(It.IsAny<Category>()));
            _unitOfWorkMock.Setup(uow => uow.Category).Returns(_categoryRepositoryMock.Object);
            _unitOfWorkMock.Setup(uow => uow.Save());
            var controller = new CategoryController(_unitOfWorkMock.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>()); // Mock of TempData

            // Act
            var result = controller.Create(newCategory);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult!.ActionName.Should().Be("Index");
            _categoryRepositoryMock.Verify(repo => repo.Add(It.Is<Category>(c => c == newCategory)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void CategoryController_Create_Post_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            var invalidCategory = new Category { Id = 4, Name = "", DisplayOrder = 101 }; // Invalid: Name required, DisplayOrder out of range
            _unitOfWorkMock.Setup(uow => uow.Category).Returns(_categoryRepositoryMock.Object);
            var controller = new CategoryController(_unitOfWorkMock.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>()); // Mock of TempData
            controller.ModelState.AddModelError("Name", "Required");
            controller.ModelState.AddModelError("DisplayOrder", "Out of range");

            // Act
            var result = controller.Create(invalidCategory);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _categoryRepositoryMock.Verify(repo => repo.Add(It.IsAny<Category>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
        /*
        [Fact]
        public void CategoryController_Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var newCategory = new Category { Id = 3, Name = "Comedy", DisplayOrder = 3 };
            _categoryRepositoryMock.Setup(repo => repo.Add(It.IsAny<Category>()));
            _unitOfWorkMock.Setup(uow => uow.Category).Returns(_categoryRepositoryMock.Object);
            _unitOfWorkMock.Setup(uow => uow.Save());
            var controller = new CategoryController(_unitOfWorkMock.Object);
            // Act
            var result = controller.Create(newCategory);
            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult!.ActionName.Should().Be("Index");
            _categoryRepositoryMock.Verify(repo => repo.Add(It.Is<Category>(c => c == newCategory)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }
        */
    }
}
