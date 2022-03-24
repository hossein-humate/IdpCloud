using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("IdpCloud.Tests")]
namespace IdpCloud.ServiceProvider.EntityService.Security
{
    /// <inheritdoc />
    internal class ResetPasswordRepository : IResetPasswordRepository
    {
        protected readonly DbSet<ResetPassword> _resetPasswords;
        public ResetPasswordRepository(EfCoreContext databaseContext)
        {
            _resetPasswords = databaseContext.Set<ResetPassword>();
        }

        public async Task<ResetPassword> AddAsync(
            ResetPassword resetPassword,
            CancellationToken cancellationToken = default)
        {
            await _resetPasswords.AddAsync(resetPassword, cancellationToken);
            return resetPassword;
        }

        public async Task<ResetPassword> FindByUserIdAndSecretAsync(
            Guid userId,
            string secret,
            CancellationToken cancellationToken = default)
        {
            return await _resetPasswords.Where(r =>
                r.User.UserId == userId
                && r.Secret == secret
                && r.Active)
                .Include(r => r.User)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public ResetPassword Update(ResetPassword resetPassword)
        {
            _resetPasswords.Update(resetPassword);
            return resetPassword;
        }
    }
}
