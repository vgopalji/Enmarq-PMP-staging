﻿using CareStream.Models;
using CareStream.Utility;
using CareStream.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using CareStream.LoggerService;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace CareStream.Test.Controllers
{
    public class UserAttributeControllerTest
    {
        private UserAttributeController _userAttributeController;
        private Mock<IUserAttributeService> mockUserAttributeServiceRepo;

        #region Test Cases

        [Fact]
        public async Task List_ReturnsAViewResult_WithUserAttributeModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockUserAttributeServiceRepo.Setup(x => x.GetUserAttribute()).Returns(Task.FromResult(GetUserAttributeModel()));

            // Act
            var result = await _userAttributeController.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as UserAttributeModel;
            Assert.Equal("test", model.Id);
        }

        [Fact]
        public async Task Create_ReturnsAViewResult_ByCreatingSelectList()
        {
            DoGroupMembersControllerInitialSetup();

            // Act
            var result = await _userAttributeController.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Upsert_ReturnsARedirectToActionResult_ByUpdatingModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockUserAttributeServiceRepo.Setup(x => x.UpsertUserAttributes(It.IsAny<ExtensionModel>())).Returns(Task.FromResult(true));

            // Act
            var result = await _userAttributeController.Upsert(GetExtensionModel());

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        #endregion

        #region Private Methods

        private void DoGroupMembersControllerInitialSetup()
        {
            mockUserAttributeServiceRepo = new Mock<IUserAttributeService>();
            var mockLoggerRepo = new Mock<ILoggerManager>();

            _userAttributeController = new UserAttributeController(mockUserAttributeServiceRepo.Object, mockLoggerRepo.Object);
        }

        private UserAttributeModel GetUserAttributeModel()
        {
            return new UserAttributeModel
            {
                Id = "test",
                ExtensionSchemas = new List<ExtensionSchema>
                {
                    new ExtensionSchema
                    {
                        DataType= "testType",
                        Name="test"
                    }
                }
            };
        }

        private ExtensionModel GetExtensionModel()
        {
            return new ExtensionModel
            {
                DataType = "testType",
                Name = "test",
                Description = "test desc",
                TargetObjects = new List<string> { "test target" }
            };
        }

        #endregion
    }
}
