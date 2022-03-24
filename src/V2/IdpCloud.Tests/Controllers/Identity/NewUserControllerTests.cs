using AutoMapper;
using IdpCloud.REST.Areas.Identity.Controllers;
using IdpCloud.REST.Areas.SSO.Controllers;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using IdpCloud.Sdk.Model.SSO.Request.User;
using IdpCloud.Sdk.Model.SSO.Response.Organisation;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Controllers.Identity
{
    /// <summary>
    /// Test suite for <see cref="UserController"/>.
    /// </summary>
    public class NewUserControllerTests
    {
        private readonly NewUserController _userController;
        private readonly Mock<IUserService> _mockUserService = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSessionService = new();
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialises an instance of <see cref="NewUserControllerTests"/>.
        /// </summary>
        public NewUserControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();
            _mapper = mapper;

            _userController = new NewUserController(
                _mockUnitOfWork.Object,
                _mockConfiguration.Object,
                mapper,
                _mockCurrentUserSessionService.Object,
                _mockUserService.Object
                );
        }

        /// <summary>
        /// Asserts that when calling <see cref="UserController.CreateAsync(CreateUserRequest, CancellationToken)"/> 
        /// that should contain a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task CreateAsync_ReturnCreatedOrganisationInResponse()
        {
            //Arrange
            var request = new CreateUserRequest()
            {
                Username = "TestUserName",
                Firstname = "TestFirstName",
                Lastname = "TestLastName",
                Email = "TestEmail",
                Mobile = "123654789",
                OrganisationId = 1,
                RoleId = "TestRoleId"
            };

            var userID = Guid.NewGuid();

            var user = new DataProvider.Entity.Identity.User
            {
                UserId = userID,
                Firstname = "TestFirstName",
                Lastname = "TestLastName",
                Username = "TestUserName",
                Email = "TestEmail",
                Mobile = "07459791360",
                OrganisationId = 1
            };

            _mockUserService
                .Setup(x => x.Create(request, default))
                .ReturnsAsync(user);

            var mappedUser = _mapper.Map<NewUser>(user);

            //Act
            var actionResult = await _userController.CreateAsync(request, default);

            //Assert
            _mockUserService.Verify(m => m.Create(It.IsAny<CreateUserRequest>(), default), Times.Once);

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            var createResponse = Assert.IsType<CreateUserResponse>(objectResult.Value);
            Assert.Equal(RequestResult.RequestSuccessful, createResponse.ResultCode);
            Assert.Equal("Request Completed Successfully.", createResponse.Message);

            Assert.Equal(mappedUser.FirstName, createResponse.User.FirstName);
            Assert.Equal(mappedUser.LastName, createResponse.User.LastName);
            Assert.Equal(mappedUser.Username, createResponse.User.Username);
            Assert.Equal(mappedUser.Email, createResponse.User.Email);
            Assert.Equal(mappedUser.Mobile, createResponse.User.Mobile);
            Assert.Equal(mappedUser.OrganisationId, createResponse.User.OrganisationId);
            Assert.Equal(mappedUser.UserId, createResponse.User.UserId);
        }

        /// <summary>
        /// Asserts that when calling <see cref="UserController.CreateAsync(CreateUserRequest, CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status500InternalServerError"/> response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task CreateAsync_Throws()
        {
            //Arrange
            _mockUserService
                .Setup(x => x.Create(It.IsAny<CreateUserRequest>(), default))
                 .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _userController.CreateAsync(default);

            //Assert
            _mockUserService.Verify(m => m.Create(It.IsAny<CreateUserRequest>(), default), Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="UserController.Get(int?, int?, string, string, CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status200OK"/> response, the response model should
        /// contain resources of <see cref="Organisation"/> inside <see cref="ListBaseResponse{Organisation}"/> with 
        /// total items and pages number
        /// </summary>
        /// <param name="pageIndex">represent page index to get list result</param>
        /// <param name="pageSize">represent page size to calculate database records by this page size</param>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Theory]
        [InlineData(1, 10, 50)]
        [InlineData(5, 10, 45)]
        [InlineData(6, 10, 50)]
        public async Task Get_ShouldReturnOrganisationPagedResult_WhenGivenPageIndexAndSize(int pageIndex, int pageSize, int orgInDB)
        {
            //Arrange
            var result = TempPagedUserResult(pageIndex, pageSize, orgInDB);

            _mockUserService.Setup(x => x.GetAll(It.IsAny<UserPaginationParam>(), default)).ReturnsAsync(result);

            //Act
            var actionResult = await _userController.Get(pageIndex, pageSize, string.Empty, string.Empty, default);

            //Assert
            _mockUserService.Verify(m => m.GetAll(It.Is<UserPaginationParam>(x =>
                x.PageIndex == pageIndex && x.PageSize == pageSize), default), Times.Once);

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var listBaseResponse = Assert.IsType<ListBaseResponse<UserSummary>>(objectResult.Value);
            Assert.Equal(RequestResult.RequestSuccessful, listBaseResponse.ResultCode);
            Assert.Equal("Request Completed Successfully.", listBaseResponse.Message);
            Assert.Equal(orgInDB, listBaseResponse.TotalItems);
            if (orgInDB % pageSize == 0)
            {
                Assert.Equal(orgInDB / pageSize, listBaseResponse.TotalPages);
            }
            else
            {
                Assert.Equal((orgInDB / pageSize) + 1, listBaseResponse.TotalPages);
            }
            Assert.Equal(result.Items.Count(), listBaseResponse.Resources.Count);
        }

        /// <summary>
        /// Asserts that when calling <see cref="UserController.Get(int?, int?, string, string, CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status500InternalServerError"/> response, the response model should
        /// contain <see cref="BaseResponse"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Get_Throws()
        {
            //Arrange
            _mockUserService.Setup(x => x.GetAll(It.IsAny<UserPaginationParam>(), default)).ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _userController.Get(default, default, default, default, default);

            //Assert
            _mockUserService.Verify(x => x.GetAll(It.IsAny<UserPaginationParam>(), default), Times.Once);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="NewUserController.Update(UpdateUserRequest, CancellationToken)"/> 
        /// calls <see cref="UserService.Update(UpdateUserRequest, CancellationToken)"/>update the user details in database
        /// that should contain a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Update_ReturnUpdatedUserInResponse()
        {
            //Arrange
            var updateRequest = new UpdateUserRequest();

            var user = new DataProvider.Entity.Identity.User
            {
                UserId = Guid.NewGuid(),
                Firstname = "TestFirstName",
                Lastname = "TestLastName",
                Email = "Test@Test.com",
                Username = "TestUserName",
                Mobile = "784596133",
                OrganisationId = 1,
            };

            _mockUserService
                .Setup(m => m.Update(It.IsAny<UpdateUserRequest>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var mappedUser = _mapper.Map<NewUser>(user);

            //Act
            var actionResult  = await _userController.Update(updateRequest, default);

            //Assert
            _mockUserService.Verify(m => m.Update(It.IsAny<UpdateUserRequest>(), default), Times.Once());

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var createResponse = Assert.IsType<UpdateUserResponse>(objectResult.Value);
            Assert.Equal(RequestResult.RequestSuccessful, createResponse.ResultCode);
            Assert.Equal("Request Completed Successfully.", createResponse.Message);

            Assert.Equal(mappedUser.UserId, createResponse.User.UserId);
            Assert.Equal(mappedUser.Username, createResponse.User.Username);
            Assert.Equal(mappedUser.Email, createResponse.User.Email); 
            Assert.Equal(mappedUser.FirstName, createResponse.User.FirstName);
            Assert.Equal(mappedUser.LastName, createResponse.User.LastName);
            Assert.Equal(mappedUser.Mobile, createResponse.User.Mobile);
            Assert.Equal(mappedUser.OrganisationId, createResponse.User.OrganisationId);
        }

        /// <summary>
        /// Asserts that when calling <see cref="NewUserController.Update(UpdateUserRequest, CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status500InternalServerError"/> response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Update_Throws()
        {
            //Arrange
            _mockUserService
                .Setup(x => x.Update(It.IsAny<UpdateUserRequest>(), default))
                 .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _userController.Update(default);

            //Assert
            _mockUserService.Verify(m => m.Update(It.IsAny<UpdateUserRequest>(), default), Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        private static UserPaginationResult TempPagedUserResult(int pageIndex, int pageSize, int count)
        {
            var users = new List<UserSummary>();
            for (int i = 0; i < count; i++)
            {
                users.Add(new UserSummary
                {
                    UserId = Guid.NewGuid(),
                    Name = "User" + i
                });
            }

            var skip = (pageIndex - 1) * pageSize;

            var result = users.Skip(skip).Take(pageSize);

            return new UserPaginationResult { Items = result, TotalItems = count };
        }

    }
}
