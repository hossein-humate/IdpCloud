using IdpCloud.Common;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Tests.InMemory;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IdpCloud.Tests.Helper
{
    /// <summary>
    /// Test suite for <see cref="ExpressionHelper{Organisation}"/>.
    /// </summary>
    public class ExpressionHelperTests : TestBase
    {
        private readonly ExpressionHelper _expressionHelper;
        private readonly EfCoreContext _efDbContext;
        private List<Organisation> _organisations = new();

        /// <summary>
        /// Initialises an instance of <see cref="OrganisationRepositoryTests"/>.
        /// </summary>
        public ExpressionHelperTests()
        {
            _expressionHelper = new ExpressionHelper();
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            var orgs = GenerateOrganisation(10);
            _efDbContext.Organisations.AddRange(orgs);
            _efDbContext.SaveChanges();
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "ExpressionHelper.TranslateFilterToLambda{T}(string)" />
        /// Search for Organisations those are matched to the given filter query
        /// </summary>
        [Fact]
        public void CompileFilterExpression_WhenGivenLowerThanOrContain()
        {
            //Arrange
            var filter = "OrganisationId lt 5 _Or Name cn map9";

            //Act
            var predicate = _expressionHelper.TranslateFilterToLambda<Organisation>(filter);
            var result = _efDbContext.Organisations.Where(predicate);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count());
            Assert.Contains(result, x => x.Name.Contains("map9"));
            Assert.Contains(result, x => x.OrganisationId < 5 && !x.Name.Contains("map9"));
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "ExpressionHelper.TranslateFilterToLambda{T}(string)" />
        /// Search for Organisations those are matched to the given filter query
        /// </summary>
        [Fact]
        public void CompileFilterExpression_WhenGivenGreaterThanAndLowerThanEqual()
        {
            //Arrange
            var filter = "OrganisationId gt 3 _And OrganisationId le 5";

            //Act
            var predicate = _expressionHelper.TranslateFilterToLambda<Organisation>(filter);
            var result = _efDbContext.Organisations.Where(predicate);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.All(x => x.OrganisationId > 1));
            Assert.Contains(result, x => x.OrganisationId == 4);
            Assert.Contains(result, x => x.OrganisationId > 3 && x.OrganisationId <= 5);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "ExpressionHelper{T}.TranslateFilterToLambda(string)" />
        /// Search for Organisations those are matched to the given filter query
        /// </summary>
        [Fact]
        public void CompileFilterExpression_WhenGivenGreaterThanEqualAndEqual()
        {
            //Arrange
            var filter = "OrganisationId ge 3 _And Name eq Basemap2";

            //Act
            var predicate = _expressionHelper.TranslateFilterToLambda<Organisation>(filter);
            var result = _efDbContext.Organisations.Where(predicate);

            //Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.All(x => x.Name == "Basemap2"));
            Assert.Contains(result, x => x.OrganisationId >= 3);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "ExpressionHelper{T}.ApplySort(IList{T}, string)" />
        /// Search for Organisations those are matched to the given filter query
        /// </summary>
        [Fact]
        public void ApplyOrderOrder_WhenGivenOrderBy()
        {
            //Arrange
            var orderBy = "OrganisationId,Name desc";

            //Act
            var result = _expressionHelper.ApplySort(_efDbContext.Organisations.ToList(), orderBy);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Basemap0", result.First().Name);
            Assert.Equal("Basemap9", result.Last().Name);
            Assert.Equal(1, result.First().OrganisationId);
            Assert.Equal(10, result.Last().OrganisationId);
        }


        private static List<Organisation> GenerateOrganisation(int count)
        {
            var organisations = new List<Organisation>();
            for (int i = 0; i < count; i++)
            {
                organisations.Add(new Organisation
                {
                    Name = "Basemap" + i,
                    BillingEmail = $"test{i}@basemap.co.uk",
                    BillingAddress = $"NO{i} Street{i}",
                    Phone = $"+44-{i}",
                    VatNumber = $"UK12345667{i}"
                });
            }
            return organisations;
        }
    }
}
