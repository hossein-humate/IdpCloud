using IdpCloud.Common;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using IdpCloud.Sdk.Model.SSO.Response.Organisation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <inheritdoc />
    public class OrganisationRepository : IOrganisationRepository
    {
        protected readonly DbSet<Organisation> _Organisations;

        /// <summary>
        /// Initialises an instance of <see cref="OrganisationRepository"/> And inject the dbContext.
        /// </summary>
        public OrganisationRepository(EfCoreContext efCoreContext)
        {
            _Organisations = efCoreContext.Set<Organisation>();
        }

        /// <inheritdoc />
        public async Task<Organisation> Add(Organisation Organisation)
        {
            return (await _Organisations.AddAsync(Organisation)).Entity;
        }

        /// <inheritdoc />
        public async Task<OrganisationPaginationResult> GetAll(
            OrganisationPaginationParam param,
            CancellationToken cancellationToken = default)
        {
            var expressionHelper = new ExpressionHelper();
            var organisations = _Organisations.Where(o => o.DeleteDate == null);
            organisations = organisations.Where(expressionHelper.FilterToLambda<Organisation>(param.Filter));
            var totalItems = organisations.Count();
            var skip = (param.PageIndex - 1) * param.PageSize;
            var result = await organisations.Skip(skip)
                .Take(param.PageSize)
                .Select(o => new OrganisationSummary
                {
                    OrganisationId = o.OrganisationId,
                    Name = o.Name,
                    BillingEmail = o.BillingEmail,
                    Phone = o.Phone,
                    BillingAddress = o.BillingAddress,
                    VatNumber = o.VatNumber,
                    UserCounts = o.Users.Count,
                    CreationDate = o.CreateDate,
                    UpdateDate = o.UpdateDate,
                    LastLoginDate = o.Users.OrderByDescending(u => u.LastLoginDate).FirstOrDefault().LastLoginDate,
                })
                .ToListAsync(cancellationToken);
            result = expressionHelper.ApplySort(result, param.OrderBy ?? "Name");

            return new OrganisationPaginationResult
            {
                Items = result,
                TotalItems = totalItems
            };
        }

        /// <inheritdoc />
        public Organisation Update(Organisation organisation)
        {
            organisation.UpdateDate = DateTime.UtcNow;
            return (_Organisations.Update(organisation)).Entity;
        }

        /// <inheritdoc />
        public async Task<Organisation> FindById(int organisationId, CancellationToken cancellationToken = default)
        {
            var organisation = await _Organisations.Where(o =>
                    o.OrganisationId == organisationId && o.DeleteDate == null)
                .Include(o => o.Users)
                .SingleOrDefaultAsync(cancellationToken);
            return organisation;
        }

        /// <inheritdoc />
        public void Delete(Organisation organisation)
        {
            organisation.DeleteDate = DateTime.UtcNow;
            _Organisations.Update(organisation);
        }
    }
}
