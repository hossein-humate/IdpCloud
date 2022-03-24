using AutoMapper;
using IdpCloud.REST.Areas.SSO.Structure;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Reflection;

namespace IdpCloud.Tests.Controllers.SSO
{
    /// <summary>
    /// Test suite for <see cref="SsoBaseController"/>.
    /// </summary>
    public class SsoBaseControllerTests
    {
        private readonly SsoBaseController _ssoBaseController;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSession = new();

        /// <summary>
        /// Initialises an instance of <see cref="SsoBaseControllerTests"/>.
        /// </summary>
        public SsoBaseControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();

            _ssoBaseController = new SsoBaseController(
                _mockUnitOfWork.Object,
                _mockConfiguration.Object,
                mapper,
                _mockCurrentUserSession.Object);
        }
    }
}
