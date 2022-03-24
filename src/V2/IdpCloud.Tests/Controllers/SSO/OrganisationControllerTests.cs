using AutoMapper;
using IdpCloud.REST.Areas.SSO.Controllers;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using IdpCloud.Sdk.Model.SSO.Response.Organisation;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using System.Threading;

namespace IdpCloud.Tests.Controllers.SSO
{
    /// <summary>
    /// Test suite for <see cref="OrganisationController"/>.
    /// </summary>
    public class OrganisationControllerTests
    {
        private readonly OrganisationController _organisationController;
        private readonly Mock<IUnitOfWork> _mockUnitofWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSession = new();
        private readonly Mock<IOrganisationService> _mockOrganisationService = new();
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialises an instance of <see cref="OrganisationControllerTests"/>.
        /// </summary>
        public OrganisationControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();
            _mapper = mapper;

            _organisationController = new OrganisationController(
                _mockUnitofWork.Object,
                _mockConfiguration.Object,
                mapper,
                _mockCurrentUserSession.Object,
                _mockOrganisationService.Object);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.CreateAsync(CreateRequest, System.Threading.CancellationToken)"/> 
        /// that should contain a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task CreateAsync_ReturnCreatedOrganisationInResponse()
        {
            //Arrange
            var oragnisation = new DataProvider.Entity.Identity.Organisation
            {
                OrganisationId = 1,
                Name = "TestOrganisation",
                BillingEmail = "TestEmail",
                BillingAddress = "TestBillingAddress",
                VatNumber = "TestVatNumber",
                Phone = "TestPhone",
            };
            _mockOrganisationService
                .Setup(x => x.Create(It.IsAny<CreateRequest>(), default))
                .ReturnsAsync(oragnisation);
            var mappedOragnisation = _mapper.Map<Organisation>(oragnisation);

            //Act
            var actionResult = await _organisationController.CreateAsync(default);

            //Assert
            _mockOrganisationService.Verify(m => m.Create(It.IsAny<CreateRequest>(), default), Times.Once);

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var createResponse = Assert.IsType<CreateResponse>(objectResult.Value);
            Assert.Equal(RequestResult.RequestSuccessful, createResponse.ResultCode);
            Assert.Equal("Request Completed Successfully.", createResponse.Message);
            Assert.Equal(mappedOragnisation.OrganisationId, createResponse.Organisation.OrganisationId);
            Assert.Equal(mappedOragnisation.Name, createResponse.Organisation.Name);
            Assert.Equal(mappedOragnisation.BillingEmail, createResponse.Organisation.BillingEmail);
            Assert.Equal(mappedOragnisation.BillingAddress, createResponse.Organisation.BillingAddress);
            Assert.Equal(mappedOragnisation.VatNumber, createResponse.Organisation.VatNumber);
            Assert.Equal(mappedOragnisation.Phone, createResponse.Organisation.Phone);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.CreateAsync(CreateRequest, System.Threading.CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status500InternalServerError"/> response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task CreateAsync_Throws()
        {
            //Arrange
            _mockOrganisationService
                .Setup(x => x.Create(It.IsAny<CreateRequest>(), default))
                 .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _organisationController.CreateAsync(default);

            //Assert
            _mockOrganisationService.Verify(m => m.Create(It.IsAny<CreateRequest>(), default), Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.Get(int?, int?, string, string, CancellationToken)"/> 
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
            OrganisationPaginationResult result = TempPagedOrganisationResult(pageIndex, pageSize, orgInDB);

            _mockOrganisationService
                .Setup(x => x.GetAll(It.IsAny<OrganisationPaginationParam>(), default))
                 .ReturnsAsync(result);

            //Act
            var actionResult = await _organisationController.Get(pageIndex, pageSize,string.Empty,string.Empty, default);

            //Assert
            _mockOrganisationService.Verify(m => m.GetAll(It.Is<OrganisationPaginationParam>(x =>
                x.PageIndex == pageIndex && x.PageSize == pageSize), default), Times.Once);

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var listBaseResponse = Assert.IsType<ListBaseResponse<OrganisationSummary>>(objectResult.Value);
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
        /// Asserts that when calling <see cref="OrganisationController.Get(int?, int?, string, string, CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status500InternalServerError"/> response, the response model should
        /// contain <see cref="BaseResponse"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Get_Throws()
        {
            //Arrange
            _mockOrganisationService
               .Setup(x => x.GetAll(It.IsAny<OrganisationPaginationParam>(), default))
                .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _organisationController.Get(default, default,default,default, default);

            //Assert
            _mockOrganisationService.Verify(x => x.GetAll(It.IsAny<OrganisationPaginationParam>(), default), Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.Delete(int, System.Threading.CancellationToken)"/> 
        /// that should contain a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Delete_ReturnSuccessfulResult()
        {
            //Arrange
            _mockOrganisationService
                .Setup(x => x.Delete(It.IsAny<int>(), default))
                .ReturnsAsync(RequestResult.RequestSuccessful);

            //Act
            var actionResult = await _organisationController.Delete(default, default);

            //Assert
            _mockOrganisationService.Verify(m => m.Delete(It.IsAny<int>(), default), Times.Once);

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.RequestSuccessful, baseResponse.ResultCode);
            Assert.Equal("Request Completed Successfully.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.Delete(int, System.Threading.CancellationToken)"/> 
        /// that should contain a non-null
        /// <see cref="BaseResponse"/> object and the controller will return a <see cref="BadRequestResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Delete_ReturnBadRequestOrganisationHasUser()
        {
            //Arrange
            _mockOrganisationService
                .Setup(x => x.Delete(It.IsAny<int>(), default))
                .ReturnsAsync(RequestResult.OrganisationHasUser);

            //Act
            var actionResult = await _organisationController.Delete(default, default);

            //Assert
            _mockOrganisationService.Verify(m => m.Delete(It.IsAny<int>(), default), Times.Once);

            var objectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.OrganisationHasUser, baseResponse.ResultCode);
            Assert.Equal("Cannot complete operation, Organisation has one or more Users.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.Delete(int, System.Threading.CancellationToken)"/> 
        /// that should contain a non-null
        /// <see cref="BaseResponse"/> object and the controller will return a <see cref="BadRequestResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Delete_ReturnBadRequestNotExistCannotContinue()
        {
            //Arrange
            _mockOrganisationService
                .Setup(x => x.Delete(It.IsAny<int>(), default))
                .ReturnsAsync(RequestResult.NotExistCannotContinue);

            //Act
            var actionResult = await _organisationController.Delete(default, default);

            //Assert
            _mockOrganisationService.Verify(m => m.Delete(It.IsAny<int>(), default), Times.Once);

            var objectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.NotExistCannotContinue, baseResponse.ResultCode);
            Assert.Equal("Not Found/Exist by the given data, Cannot continue the operation.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.Delete(int, System.Threading.CancellationToken)"/> 
        /// the controller will return a <see cref="StatusCodes.Status500InternalServerError"/> response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Delete_Throws()
        {
            //Arrange
            _mockOrganisationService
                .Setup(x => x.Delete(It.IsAny<int>(), default))
                 .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _organisationController.Delete(default, default);

            //Assert
            _mockOrganisationService.Verify(m => m.Delete(It.IsAny<int>(), default), Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        private static OrganisationPaginationResult TempPagedOrganisationResult(int pageIndex, int pageSize, int count)
        {
            var organisations = new List<OrganisationSummary>();
            for (int i = 0; i < count; i++)
            {
                organisations.Add(new OrganisationSummary
                {
                    OrganisationId = i,
                    Name = "Basemap" + i
                });
            }

            var skip = (pageIndex - 1) * pageSize;

            var result = organisations.Skip(skip).Take(pageSize);

            return new OrganisationPaginationResult { Items = result, TotalItems = count };
        }


        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.Edit(UpdateRequest, CancellationToken)"/> 
        /// that should contain a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Update_ReturnUpdatedOrganisationInResponse()
        {
            //Arrange
            var oragnisation = new DataProvider.Entity.Identity.Organisation
            {
                OrganisationId = 1,
                Name = "TestOrganisation",
                BillingEmail = "TestEmail",
                BillingAddress = "TestBillingAddress",
                VatNumber = "TestVatNumber",
                Phone = "TestPhone",
            };

            _mockOrganisationService
                .Setup(x => x.Update(It.IsAny<UpdateRequest>(), default))
                .ReturnsAsync(oragnisation);

            var mappedOragnisation = _mapper.Map<Organisation>(oragnisation);

            //Act
            var actionResult = await _organisationController.Update(default);

            //Assert
            _mockOrganisationService.Verify(m => m.Update(It.IsAny<UpdateRequest>(), default), Times.Once);

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var createResponse = Assert.IsType<UpdateResponse>(objectResult.Value);
            Assert.Equal(RequestResult.RequestSuccessful, createResponse.ResultCode);
            Assert.Equal("Request Completed Successfully.", createResponse.Message);
            Assert.Equal(mappedOragnisation.OrganisationId, createResponse.Organisation.OrganisationId);
            Assert.Equal(mappedOragnisation.Name, createResponse.Organisation.Name);
            Assert.Equal(mappedOragnisation.BillingEmail, createResponse.Organisation.BillingEmail);
            Assert.Equal(mappedOragnisation.BillingAddress, createResponse.Organisation.BillingAddress);
            Assert.Equal(mappedOragnisation.VatNumber, createResponse.Organisation.VatNumber);
            Assert.Equal(mappedOragnisation.Phone, createResponse.Organisation.Phone);
        }


        /// <summary>
        /// Asserts that when calling <see cref="OrganisationController.Update(UpdateRequest, CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status500InternalServerError"/> response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Update_Throws()
        {
            //Arrange
            _mockOrganisationService
                .Setup(x => x.Update(It.IsAny<UpdateRequest>(), default))
                 .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _organisationController.Update(default);

            //Assert
            _mockOrganisationService.Verify(m => m.Update(It.IsAny<UpdateRequest>(), default), Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

    }
}
