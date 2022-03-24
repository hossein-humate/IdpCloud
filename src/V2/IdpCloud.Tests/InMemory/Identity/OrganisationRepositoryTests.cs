using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using IdpCloud.Sdk.Model.SSO.Response.Organisation;
using IdpCloud.ServiceProvider.EntityService.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.Identity
{
    /// <summary>
    /// Test suite for <see cref="OrganisationRepository"/>.
    /// </summary>
    public class OrganisationRepositoryTests : TestBase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly EfCoreContext _efDbContext;
        private List<Organisation> _organisations = new();

        /// <summary>
        /// Initialises an instance of <see cref="OrganisationRepositoryTests"/>.
        /// </summary>
        public OrganisationRepositoryTests()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _organisationRepository = new OrganisationRepository(_efDbContext);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "OrganisationRepository.Add(Organisation)" />
        /// returns Organisation after successfully added to Database with expected OrganisationId
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Add_ShouldAddAndReturnOrganisation_WhenGivenOrganisation()
        {
            //Arrange
            _organisations = GetOrganisations();
            _efDbContext.Organisations.AddRange(_organisations);
            _efDbContext.SaveChanges();
            var organisation = new Organisation
            {
                Name = "Basemap4",
                BillingAddress = "TestAddress",
                BillingEmail = "TestEmail",
                VatNumber = "TestVat",
                Phone = "TestPhone"
            };

            //Act
            var resultOrganisation = await _organisationRepository.Add(organisation);

            //Assert

            Assert.NotNull(resultOrganisation);
            Assert.Equal(organisation.Name, resultOrganisation.Name);
            Assert.Equal(organisation.BillingAddress, resultOrganisation.BillingAddress);
            Assert.Equal(organisation.BillingEmail, resultOrganisation.BillingEmail);
            Assert.Equal(organisation.Phone, resultOrganisation.Phone);
            Assert.Equal(organisation.VatNumber, resultOrganisation.VatNumber);
            Assert.Equal(4, resultOrganisation.OrganisationId);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationRepository.GetAll(OrganisationPaginationParam, System.Threading.CancellationToken)"/> 
        /// the method will return an resources of <see cref="Organisation"/> inside <see cref="OrganisationPaginationResult"/> with 
        /// total items number
        /// </summary>
        /// <param name="index">represent page index to get list result</param>
        /// <param name="size">represent page size to calculate database records by this page size</param>
        /// <param name="orgInDB">represent entites record inside the database</param>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Theory]
        [InlineData(1, 10, 50)]
        [InlineData(5, 10, 45)]
        [InlineData(6, 10, 50)]
        public async Task GetAll_ShouldReturnPagedOrganisations_WhenGivenIndexAndSize(int index, int size, int orgInDB)
        {
            //Arrange
            _organisations = GenerateOrganisation(orgInDB);
            _efDbContext.Organisations.AddRange(_organisations);
            _efDbContext.SaveChanges();

            //Act
            var resultOrganisations = await _organisationRepository.GetAll(new OrganisationPaginationParam
            {
                PageIndex = index,
                PageSize = size,
            });

            //Assert
            Assert.NotNull(resultOrganisations);
            if (index == 1)
            {
                Assert.Equal(3, resultOrganisations.Items.FirstOrDefault()?.UserCounts);
            }
            if (orgInDB % size == 0 && orgInDB / size >= index)
            {
                Assert.Equal(size, resultOrganisations.Items.Count());
            }
            Assert.Equal(orgInDB, resultOrganisations.TotalItems);
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationRepository.GetAll(OrganisationPaginationParam, System.Threading.CancellationToken)"/> 
        /// the method will return an resources of <see cref="OrganisationSummary"/> inside <see cref="OrganisationPaginationResult"/> with 
        /// total items number
        /// </summary>
        /// <param name="filter">represent the filter query string value</param>
        /// <param name="orgInDB">represent entites record inside the database</param>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Theory]
        [InlineData("OrganisationId gt 5 _And organisationid lt 10", 20)]
        [InlineData("name eq Basemap1 _Or Name eq Basemap5", 10)]
        [InlineData("name cn map1 _Or organisationId eq 2", 5)]
        public async Task GetAll_ShouldReturnFilteredOrganisations_WhenGivenFilterQuery(string filter, int orgInDB)
        {
            //Arrange
            _organisations = GenerateOrganisation(orgInDB);
            _efDbContext.Organisations.AddRange(_organisations);
            _efDbContext.SaveChanges();

            //Act
            var resultOrganisations = await _organisationRepository.GetAll(new OrganisationPaginationParam
            {
                PageIndex = 1,
                PageSize = orgInDB,
                Filter = filter
            });

            //Assert
            Assert.NotNull(resultOrganisations);
            if (orgInDB == 20)
            {
                Assert.Equal(4, resultOrganisations.Items.Count());
                Assert.Contains(resultOrganisations.Items, x => x.OrganisationId > 5 && x.OrganisationId < 10);
            }
            if (orgInDB == 10)
            {
                Assert.Equal(2, resultOrganisations.Items.Count());
                Assert.Contains(resultOrganisations.Items, x => x.Name == "Basemap1" || x.Name == "Basemap5");
            }
            if (orgInDB == 5)
            {
                Assert.Single(resultOrganisations.Items);
                Assert.Contains(resultOrganisations.Items, x => x.Name.Contains("map1") || x.OrganisationId == 2);
            }
        }

        /// <summary>
        /// Asserts that when calling <see cref="OrganisationRepository.GetAll(OrganisationPaginationParam, System.Threading.CancellationToken)"/> 
        /// the method will return an resources of <see cref="OrganisationSummary"/> inside <see cref="OrganisationPaginationResult"/> with 
        /// total items number
        /// </summary>
        /// <param name="orderby">represent the orderby query string value</param>
        /// <param name="orgInDB">represent entites record inside the database</param>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Theory]
        [InlineData("OrganisationId", 20)]
        [InlineData("Name desc", 10)]
        [InlineData("OrganisationId desc,Name", 5)]
        public async Task GetAll_ShouldReturnOrderedOrganisations_WhenGivenOrderByQuery(string orderby, int orgInDB)
        {
            //Arrange
            _organisations = GenerateOrganisation(orgInDB);
            _efDbContext.Organisations.AddRange(_organisations);
            _efDbContext.SaveChanges();

            //Act
            var resultOrganisations = await _organisationRepository.GetAll(new OrganisationPaginationParam
            {
                PageIndex = 1,
                PageSize = orgInDB,
                OrderBy = orderby
            });

            //Assert
            Assert.NotNull(resultOrganisations);
            if (orgInDB == 20)
            {
                Assert.Equal(20, resultOrganisations.Items.Count());
                Assert.Equal(1, resultOrganisations.Items.First().OrganisationId);
                Assert.Equal(20, resultOrganisations.Items.Last().OrganisationId);
            }
            if (orgInDB == 10)
            {
                Assert.Equal(10, resultOrganisations.Items.Count());
                Assert.Equal("Basemap9", resultOrganisations.Items.First().Name);
                Assert.Equal("Basemap0", resultOrganisations.Items.Last().Name);
            }
            if (orgInDB == 5)
            {
                Assert.Equal(5, resultOrganisations.Items.Count());
                Assert.Equal(5, resultOrganisations.Items.First().OrganisationId);
                Assert.Equal("Basemap4", resultOrganisations.Items.First().Name);
                Assert.Equal(1, resultOrganisations.Items.Last().OrganisationId);
                Assert.Equal("Basemap0", resultOrganisations.Items.Last().Name);
            }
        }

        /// <summary>
        /// Asserts <see cref="OrganisationRepository.Update(Organisation)"/>
        /// should update organisation and return a Organisation when given valid organisation
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Update_ShouldUpdateOrgansaition_WhenGivenOrganisation()
        {
            //Arrange
            var organisation = new Organisation
            {
                Name = "Test",
                BillingAddress = "TestAddress",
                BillingEmail = "TestEmail",
                VatNumber = "TestVat",
                Phone = "TestPhone"
            };

            var org = await _organisationRepository.Add(organisation);
            org.Name = "TestOrganisation";
            org.BillingAddress = "TestBillingAddress";

            //Act
            var updateOrganisation = _organisationRepository.Update(org);

            //Assert
            Assert.NotNull(updateOrganisation);
            Assert.Equal("TestOrganisation", updateOrganisation.Name);
            Assert.Equal("TestBillingAddress", updateOrganisation.BillingAddress);
            Assert.Equal("TestEmail", updateOrganisation.BillingEmail);
            Assert.Equal("TestVat", updateOrganisation.VatNumber);
            Assert.Equal("TestPhone", updateOrganisation.Phone);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "OrganisationRepository.FindById(int, System.Threading.CancellationToken)" />
        /// returns Organisation after successfully find by given OrganisationId
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task FindById_ShouldReturnOrganisation_WhenGivenOrganisationId()
        {
            //Arrange
            _efDbContext.Organisations.AddRange(GenerateOrganisation(5));
            _efDbContext.SaveChanges();

            //Act
            var resultOrganisation = await _organisationRepository.FindById(1, default);

            //Assert
            Assert.NotNull(resultOrganisation);
            Assert.NotNull(resultOrganisation.Users);
            Assert.Equal(3, resultOrganisation.Users.Count);
            Assert.Null(resultOrganisation.DeleteDate);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "OrganisationRepository.Delete(Organisation)" />
        /// Delete Organisation find by given Organisation record
        /// </summary>
        [Fact]
        public void Delete_WhenGivenOrganisation()
        {
            //Arrange
            var orgs = GetOrganisations();
            _efDbContext.Organisations.AddRange(orgs);
            _efDbContext.SaveChanges();

            //Act
            _organisationRepository.Delete(orgs.First());

            //Assert
            Assert.NotNull(orgs.First().DeleteDate);
        }

        private static List<Organisation> GenerateOrganisation(int count)
        {
            var organisations = new List<Organisation>();
            for (int i = 0; i < count; i++)
            {
                organisations.Add(new Organisation
                {
                    Name = "Basemap" + i
                });
            }
            organisations[0].Users = new List<User>
            {
                new User{ Firstname = "Test1",Username="TestUsername1"},
                new User{ Firstname = "Test2",Username="TestUsername2"},
                new User{ Firstname = "Test3",Username="TestUsername3"},
            };
            return organisations;
        }

        private static List<Organisation> GetOrganisations()
        {
            var Organisation1 = new Organisation() { Name = "Basemap1" };
            var Organisation2 = new Organisation() { Name = "Basemap2" };
            var Organisation3 = new Organisation() { Name = "Basemap3" };
            return new List<Organisation>() { Organisation1, Organisation2, Organisation3 };
        }
    }
}
