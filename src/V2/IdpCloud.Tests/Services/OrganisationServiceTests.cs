using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services
{
    /// <summary>
    /// Test suite for <see cref="OrganisationService"/>.
    /// </summary>
    public class OrganisationServiceTests
    {
        private readonly OrganisationService _organisationService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IOrganisationRepository> _mockOrganisationRepository = new();
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialises an instance of <see cref="OrganisationServiceTests"/>.
        /// </summary>
        public OrganisationServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();
            _mapper = mapper;
            _organisationService = new OrganisationService(
                            _mockUnitOfWork.Object,
                            _mockOrganisationRepository.Object,
                            mapper);
        }

        /// <summary>
        /// Asserts when calling <see cref="OrganisationService.Create(CreateRequest, CancellationToken)"/>
        /// will populate map DTO to organisation entity and Add it in the database
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Create_ShouldCreateReturnNewAddedOrganisation_WhenGivenCreateRequest()
        {
            //Arrange
            var request = new CreateRequest
            {
                Name = "TestOrganisation",
                BillingEmail = "TestEmail",
                BillingAddress = "TestBillingAddress",
                VatNumber = "TestVatNumber",
                Phone = "TestPhone",
            };
            var organisation = _mapper.Map<Organisation>(request);

            _mockOrganisationRepository
                .Setup(m => m.Add(It.IsAny<Organisation>()))
                .ReturnsAsync(organisation);

            _mockUnitOfWork.Setup(x => x.CompleteAsync(default));

            //Act
            var addedOrganisation = await _organisationService.Create(request);

            //Assert
            _mockOrganisationRepository.Verify(m => m.Add(It.Is<Organisation>(o =>
                o.Name == request.Name
                && o.BillingEmail == request.BillingEmail
                && o.BillingAddress == request.BillingAddress
                && o.VatNumber == request.VatNumber
                && o.Phone == request.Phone)), Times.Once());

            _mockUnitOfWork.Verify(s => s.CompleteAsync(default), Times.Once());

            Assert.Equal(organisation.Name, request.Name);
            Assert.Equal(organisation.BillingEmail, request.BillingEmail);
            Assert.Equal(organisation.BillingAddress, request.BillingAddress);
            Assert.Equal(organisation.VatNumber, request.VatNumber);
            Assert.Equal(organisation.Phone, request.Phone);

            Assert.Equal(organisation.Name, addedOrganisation.Name);
            Assert.Equal(organisation.BillingEmail, addedOrganisation.BillingEmail);
            Assert.Equal(organisation.BillingAddress, addedOrganisation.BillingAddress);
            Assert.Equal(organisation.VatNumber, addedOrganisation.VatNumber);
            Assert.Equal(organisation.Phone, addedOrganisation.Phone);
        }

        /// <summary>
        /// Asserts when calling <see cref="OrganisationService.Delete(int, CancellationToken)"/>
        /// will find record in database and delete successfully
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Delete_ShouldDeleteAndReturnSuccessfull_WhenGivenValidOrganisationId()
        {
            //Arrange
            var organisation = new Organisation
            {
                OrganisationId = 1,
                Name = "TestOrganisation",
                BillingEmail = "TestEmail",
                BillingAddress = "TestBillingAddress",
                VatNumber = "TestVatNumber",
                Phone = "TestPhone",
                Users = new List<User>()
            };
            _mockOrganisationRepository
                .Setup(m => m.FindById(It.IsAny<int>(), default))
                .ReturnsAsync(organisation);

            _mockOrganisationRepository
                .Setup(m => m.Delete(It.IsAny<Organisation>()));

            _mockUnitOfWork.Setup(x => x.CompleteAsync(default));

            //Act
            var result = await _organisationService.Delete(organisation.OrganisationId, default);

            //Assert
            _mockOrganisationRepository.Verify(m => m.FindById(It.Is<int>(o =>
                o == organisation.OrganisationId), default), Times.Once());

            _mockOrganisationRepository.Verify(m => m.Delete(It.Is<Organisation>(o =>
                o.OrganisationId == organisation.OrganisationId)), Times.Once());

            _mockUnitOfWork.Verify(s => s.CompleteAsync(default), Times.Once());

            Assert.Equal(RequestResult.RequestSuccessful, result);
        }

        /// <summary>
        /// Asserts when calling <see cref="OrganisationService.Delete(int, CancellationToken)"/>
        /// will find record in database check Organsiation record not delete when already has user inside it
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Delete_ShouldReturnOrganisationHasUser_WhenGivenOrganisationHasOneOrMoreUsers()
        {
            //Arrange
            var organisation = new Organisation
            {
                OrganisationId = 1,
                Name = "TestOrganisation",
                BillingEmail = "TestEmail",
                BillingAddress = "TestBillingAddress",
                VatNumber = "TestVatNumber",
                Phone = "TestPhone",
                Users = new List<User>()
                {
                    new User{ Username = "TestUsername1",OrganisationId = 1},
                    new User{ Username = "TestUsername2",OrganisationId = 1}
                }
            };
            _mockOrganisationRepository
                .Setup(m => m.FindById(It.IsAny<int>(), default))
                .ReturnsAsync(organisation);

            //Act
            var result = await _organisationService.Delete(organisation.OrganisationId, default);

            //Assert
            _mockOrganisationRepository.Verify(m => m.FindById(It.Is<int>(o =>
                o == organisation.OrganisationId), default), Times.Once());

            _mockOrganisationRepository.Verify(m => m.Delete(It.IsAny<Organisation>()), Times.Never());

            _mockUnitOfWork.Verify(s => s.CompleteAsync(default), Times.Never());

            Assert.Equal(RequestResult.OrganisationHasUser, result);
        }

        /// <summary>
        /// Asserts when calling <see cref="OrganisationService.Delete(int, CancellationToken)"/>
        /// Cannot find organisation record by the given organisationId and delete operation not executed
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Delete_ShouldReturnNotExistCannotContinue_WhenGivenOrganisationIdIsNotValid()
        {
            //Arrange
            _mockOrganisationRepository
                .Setup(m => m.FindById(It.IsAny<int>(), default))
                .ReturnsAsync(default(Organisation));

            //Act
            var result = await _organisationService.Delete(default, default);

            //Assert
            _mockOrganisationRepository.Verify(m => m.FindById(It.IsAny<int>(), default), Times.Once());

            _mockOrganisationRepository.Verify(m => m.Delete(It.IsAny<Organisation>()), Times.Never());

            _mockUnitOfWork.Verify(s => s.CompleteAsync(default), Times.Never());

            Assert.Equal(RequestResult.NotExistCannotContinue, result);
        }
        /// <summary>
        /// Asserts when calling <see cref="OrganisationService.Update(UpdateRequest, CancellationToken)(CreateRequest, CancellationToken)"/>
        /// will populate map DTO to organisation entity and Add it in the database
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Update_ShouldUpdateExistingOrganisation_WhenGivenOrganisation()
        {
            //Arrange
            var request = new UpdateRequest
            {
                Name = "TestOrganisationUpdate",
                BillingEmail = "TestEmail",
                BillingAddress = "TestBillingAddressUpdate",
                VatNumber = "TestVatNumber",
                Phone = "TestPhone",
            };
            var oragnisation = _mapper.Map<Organisation>(request);

            _mockOrganisationRepository
               .Setup(m => m.Update(It.IsAny<Organisation>()))
               .Returns(oragnisation);

            _mockUnitOfWork.Setup(x => x.CompleteAsync(default));

            //Act
            var result = _organisationService.Update(request).Result;

            //Assert
            _mockOrganisationRepository.Verify(m => m.Update(It.Is<Organisation>(o =>
             o.Name == request.Name
             && o.BillingEmail == request.BillingEmail
             && o.BillingAddress == request.BillingAddress
             && o.VatNumber == request.VatNumber
             && o.Phone == request.Phone)), Times.Once());

            Assert.Equal(oragnisation.Name, result.Name);
            Assert.Equal(oragnisation.BillingEmail, result.BillingEmail);
            Assert.Equal(oragnisation.BillingAddress, result.BillingAddress);
            Assert.Equal(oragnisation.VatNumber, result.VatNumber);
            Assert.Equal(oragnisation.Phone, result.Phone);
            Assert.NotEqual(DateTime.MinValue, result.UpdateDate);
        }
    }
}
