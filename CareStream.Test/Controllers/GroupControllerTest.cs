using CareStream.Models;
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
    public class GroupControllerTest
    {
        private GroupController _groupController;
        private Mock<IGroupService> mockGroupServiceRepo;

        [Fact]
        public async Task Index_GetGroups_ReturnsAViewResult_WithListOfGroups()
        {
            DoGroupControllerInitialSetup();
            mockGroupServiceRepo.Setup(x => x.GetDetailGroupList()).Returns(Task.FromResult(GetGroupModels()));

            // Act
            var result = await _groupController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as List<GroupModel>;
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_GetGroupById_ReturnsAViewResult_WithGroup()
        {
            DoGroupControllerInitialSetup();
            mockGroupServiceRepo.Setup(x => x.GetGroup(It.IsAny<string>())).Returns(Task.FromResult(GetGroup()));

            // Act
            var result = await _groupController.Details("test_id");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as GroupModel;
            Assert.Equal("TestId", model.ObjectId);
        }

        [Fact]
        public async Task Create_Group_ReturnsAViewResult_WithGroupModel()
        {
            DoGroupControllerInitialSetup();
            mockGroupServiceRepo.Setup(x => x.BuildGroupOwnerMember()).Returns(Task.FromResult(GetGroupAssignedModels()));

            // Act
            var result = await _groupController.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as GroupModel;
            Assert.True(string.IsNullOrEmpty(model.ObjectId));
        }

        [Fact]
        public async Task GroupDelete_ReturnsAOkResult_ByDeletingGroup()
        {
            DoGroupControllerInitialSetup();
            mockGroupServiceRepo.Setup(x => x.RemoveGroup(It.IsAny<List<string>>())).Returns(Task.FromResult(true));

            // Act
            var result = await _groupController.GroupDelete(new List<string> { "test_id" });

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(viewResult.Value);
        }

        [Fact]
        public async Task Post_ReturnsAViewResult_ByCreatingGroup()
        {
            DoGroupControllerInitialSetup();
            mockGroupServiceRepo.Setup(x => x.CreateGroup(It.IsAny<GroupModel>())).Returns(Task.FromResult(GetGroup()));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _groupController.TempData = tempData;

            // Act
            var result = await _groupController.Post(GetGroup());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_groupController.TempData["UserMessage"]);
        }

        #region Private Methods

        private void DoGroupControllerInitialSetup()
        {
            mockGroupServiceRepo = new Mock<IGroupService>();
            var mockLoggerRepo = new Mock<ILoggerManager>();

            _groupController = new GroupController(mockGroupServiceRepo.Object, mockLoggerRepo.Object);
        }

        private List<GroupModel> GetGroupModels()
        {
            return new List<GroupModel>
            {
                new GroupModel
                {
                    Description = "TestGroup",
                    DisplayName = "Test Group Name"
                }
            };
        }

        private GroupModel GetGroup()
        {
            return new GroupModel
            {
                ObjectId = "TestId",
                Description = "TestGroup",
                DisplayName = "Test Group Name",
                GroupTypes = new List<string> { CareStreamConst.Owner, CareStreamConst.Member },
                GroupType = CareStreamConst.Owner
            };
        }

        private List<GroupAssignModel> GetGroupAssignedModels()
        {
            return new List<GroupAssignModel>
            {
                new GroupAssignModel
                {
                    AssignFor = CareStreamConst.Owner,
                    AssignList=new Dictionary<string, string>{ { "test owner", "test owner" } }
                },
                new GroupAssignModel
                {
                    AssignFor = CareStreamConst.Member,
                    AssignList=new Dictionary<string, string>{ { "test member", "test member" } }
                }
            };
        }

        #endregion
    }
}
