using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using IdpCloud.ServiceProvider.EntityService.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class OrganisationService : IOrganisationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialise an instance of <see cref="OrganisationService"/>
        /// </summary>
        public OrganisationService(IUnitOfWork unitOfWork, IOrganisationRepository organisationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _organisationRepository = organisationRepository ?? throw new ArgumentNullException(nameof(organisationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc />
        public async Task<Organisation> Create(CreateRequest request, CancellationToken cancellationToken = default)
        {
            var organisation = _mapper.Map<Organisation>(request);
            var addedOrganisation = await _organisationRepository.Add(organisation);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return addedOrganisation;
        }

        /// <inheritdoc />
        public async Task<OrganisationPaginationResult> GetAll(OrganisationPaginationParam paginationParam, CancellationToken cancellationToken = default)
        {
            return await _organisationRepository.GetAll(paginationParam, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Organisation> Update(UpdateRequest request, CancellationToken cancellationToken = default)
        {
            var organisation = _mapper.Map<Organisation>(request);
            organisation.UpdateDate = DateTime.UtcNow;
            _organisationRepository.Update(organisation);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return organisation;
        }

        /// <inheritdoc />
        public async Task<RequestResult> Delete(int organisationId, CancellationToken cancellationToken)
        {
            var result = RequestResult.OrganisationHasUser;
            var organisation = await _organisationRepository.FindById(organisationId, cancellationToken);
            if (organisation == null)
            {
                result = RequestResult.NotExistCannotContinue;
            }
            else if (!organisation.Users.Any())
            {
                _organisationRepository.Delete(organisation);
                await _unitOfWork.CompleteAsync(cancellationToken);
                result = RequestResult.RequestSuccessful;
            }
            return result;
        }
    }
}
