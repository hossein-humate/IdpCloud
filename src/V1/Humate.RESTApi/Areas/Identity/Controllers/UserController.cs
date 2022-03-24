using AutoMapper;
using Entity.Identity;
using Entity.SSO;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.RESTApi.Infrastructure.InternalService.File;
using Humate.RESTApi.Infrastructure.InternalService.Mail;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity.Request.User;
using Humate.Sdk.Model.Identity.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class UserController : IdentityBaseController
    {
        private readonly IMailService _mailService;
        private readonly IJwtAuthenticationService _jwtAuthenticationService;
        private readonly IFileStorageService _fileStorageService;
        public UserController(IJwtAuthenticationService jwtAuthenticationService, IMailService mailService,
            IFileStorageService fileStorageService, IUnitOfWork unitOfWork, IConfiguration configuration,
                IMapper mapper, IAuthenticatedUser authenticatedUser) :
                base(unitOfWork, configuration, mapper, authenticatedUser)
        {
            _jwtAuthenticationService = jwtAuthenticationService;
            _fileStorageService = fileStorageService;
            _mailService = mailService;
        }

        //[EnableQuery]
        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllResponse>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!AuthenticatedUser.AccessSoftwares.Any())
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.NotAllowedThisOperation));
                }

                var resultList = new List<SoftwareUserDto>();
                foreach (var item in AuthenticatedUser.AccessSoftwares)
                {
                    resultList.Add(new SoftwareUserDto
                    {
                        Software = Mapper.Map<Sdk.Model.Identity.Software>(item),
                        Users = Mapper.Map<IEnumerable<Sdk.Model.Identity.User>>((await UnitOfWork.UserSoftwares.FindAllAsync(
                        us => us.SoftwareId == item.SoftwareId, cancellationToken, us => us.User))
                        .Select(us => us.User)
                        .Where(u => u.DeleteDate == null))
                    });
                }
                return Ok(new
                {
                    Result = resultList.AsQueryable()
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpOptions("AvailableUsername")]
        public async Task<ActionResult<BaseResponse>> AvailableUsernameAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var username = Request.Headers["X-Username"].ToString();

                if (await UnitOfWork.Users.AnyAsNoTrackingAsync(u => u.Username == username, cancellationToken))
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.UsernameExistBefore));
                }
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetBySoftwareId/{softwareId?}")]
        public async Task<ActionResult<GetBySoftwareIdResponse>> GetBySoftwareIdAsync(Guid softwareId = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (softwareId == default)
                {
                    return Ok(new GetBySoftwareIdResponse
                    {
                        Users = Mapper.Map<IEnumerable<Sdk.Model.Identity.User>>((await UnitOfWork.UserSoftwares.FindAllAsync(us =>
                                us.SoftwareId == AuthenticatedUser.Software.SoftwareId, cancellationToken, us => us.User))
                            .Select(us => us.User).Where(u => u.DeleteDate == null))
                    });
                }
                if (AuthenticatedUser.AccessSoftwares.Any(s => s.SoftwareId == softwareId))
                {
                    return Ok(new GetBySoftwareIdResponse
                    {
                        Users = Mapper.Map<IEnumerable<Sdk.Model.Identity.User>>((await UnitOfWork.UserSoftwares.FindAllAsync(us =>
                                us.SoftwareId == softwareId, cancellationToken, us => us.User))
                           .Select(us => us.User).Where(u => u.DeleteDate == null))
                    });
                }
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.NotAllowedThisOperation));
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetUserInfo")]
        public async Task<ActionResult<GetUserInfoResponse>> GetUserInfoAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetUserInfoResponse
                {
                    User = Mapper.Map<Sdk.Model.Identity.User>(await UnitOfWork.Users.FindAsync(u =>
                        u.UserId == AuthenticatedUser.User.UserId, cancellationToken,
                        u => u.Language, u => u.Person.CountryLiving))
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }


        [HttpGet("GetById/{userId}")]
        public async Task<ActionResult<GetByIdResponse>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkUserExist = await CheckUserExistAsync(userId, cancellationToken);
                if (checkUserExist != null)
                {
                    return BadRequest(checkUserExist);
                }

                return Ok(new GetByIdResponse
                {
                    User = Mapper.Map<Sdk.Model.Identity.User>(await UnitOfWork.Users.FindAsync(u =>
                        u.UserId == userId, cancellationToken, u => u.Language, u => u.Person))
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetByUsername/{usernameOrEmail}")]
        public async Task<ActionResult<GetByUsernameResponse>> GetByUsernameAsync(string usernameOrEmail,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var user = Mapper.Map<Sdk.Model.Identity.User>(await UnitOfWork.Users.FindAsync(u =>
                    u.Email == usernameOrEmail || string.Equals(u.Username, usernameOrEmail,
                        StringComparison.CurrentCultureIgnoreCase), cancellationToken,
                    u => u.Language, u => u.Person));
                if (user == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidUserId));
                }
                return Ok(new GetByUsernameResponse
                {
                    User = user
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult<LoginUserResponse>> LoginUserAsync([FromBody] LoginUserRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var validUser = await UnitOfWork.Users.IsValidUserAsync(request.UsernameOrEmail,
                    request.Password, cancellationToken);
                if (validUser == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.WrongUsernamePassword));
                }

                validUser.LastLoginDate = DateTime.Now.ConvertToTimestamp();
                validUser.LastLoginIp = string.IsNullOrEmpty(request.ClientIp) ?
                    HttpContext.Connection.RemoteIpAddress.ToString() : request.ClientIp;
                validUser.LoginTimes += 1;
                var language = await UnitOfWork.Languages.FindAsync(l =>
                    l.LanguageId == request.LanguageId, cancellationToken);
                if (language != null)
                {
                    validUser.Language = language;
                }

                UnitOfWork.Users.Update(validUser);
                var userSession = new UserSession
                {
                    AuthType = AuthType.UserPass,
                    Status = Entity.SSO.Status.Active,
                    Ip = validUser.LastLoginIp,
                    UserAgent = string.IsNullOrEmpty(request.ClientUserAgent) ?
                        HttpContext.Request.Headers["UserAgent"].ToString() : request.ClientUserAgent,
                    SoftwareId = AuthenticatedUser.Software.SoftwareId,
                    UserId = validUser.UserId
                };
                await UnitOfWork.UserSessions.AddAsync(userSession, cancellationToken);
                await UnitOfWork.CompleteAsync(cancellationToken);

                return Ok(new LoginUserResponse
                {
                    Token = _jwtAuthenticationService.CreateTokenAuthentication(userSession.UserSessionId),
                    User = Mapper.Map<Sdk.Model.Identity.User>(validUser)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("RegisterAndLogin")]
        public async Task<ActionResult<RegisterAndLoginResponse>> RegisterAndLoginAsync(
            [FromBody] RegisterAndLoginRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = await ValidateForRegisterUserAsync(request, cancellationToken);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                var salt = Cryptography.GenerateSecret();
                if (request.CountryLivingId == null)
                {
                    request.CountryLivingId = (await UnitOfWork.Countries.FindAsync(c => c.ThreeCharacterCode == "USA",
                        cancellationToken)).CountryId;
                }
                if (request.NationalityId == null)
                {
                    request.NationalityId = (await UnitOfWork.Countries.FindAsync(c => c.ThreeCharacterCode == "USA",
                        cancellationToken)).CountryId;
                }
                var person = new Person
                {
                    Firstname = request.FirstName,
                    Lastname = request.LastName,
                    CountryLivingId = request.CountryLivingId,
                    NationalityId = request.NationalityId,
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp(),
                    SoftwareId = AuthenticatedUser.Software.SoftwareId
                };

                var user = new User
                {
                    PersonId = person.PersonId,
                    PasswordHash = request.Password.CreateHash(salt),
                    PasswordSalt = salt,
                    Email = request.Email,
                    Username = request.Username.Trim().ToLower(),
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp(),
                    RegisterIp = string.IsNullOrEmpty(request.ClientIp) ?
                        HttpContext.Connection.RemoteIpAddress.ToString() : request.ClientIp,
                    RegisterDate = DateTime.Now.ConvertToTimestamp(),
                    Status = Entity.Identity.ActiveStatus.Active,
                    EmailConfirmationExpiry = DateTime.Now.AddDays(3).ConvertToTimestamp(),
                    EmailConfirmationSecret = Cryptography.GenerateSecret(32)
                };
                var language = await UnitOfWork.Languages.FindAsync(l =>
                    l.LanguageId == request.LanguageId, cancellationToken);
                if (language != null)
                {
                    user.Language = language;
                }

                await UnitOfWork.Persons.AddAsync(person, cancellationToken);
                await UnitOfWork.Users.AddAsync(user, cancellationToken);
                await UnitOfWork.UserSoftwares.AddAsync(new UserSoftware
                {
                    SoftwareId = AuthenticatedUser.Software.SoftwareId,
                    UserId = user.UserId
                }, cancellationToken);
                var defaultRole = AuthenticatedUser.Software.Roles.FirstOrDefault(r => r.IsDefault);
                if (defaultRole != null)
                {
                    await UnitOfWork.UserRoles.AddAsync(new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = defaultRole.RoleId
                    }, cancellationToken);
                }
                var userSession = new UserSession
                {
                    AuthType = AuthType.UserPass,
                    Status = Entity.SSO.Status.Active,
                    Ip = user.RegisterIp,
                    UserAgent = string.IsNullOrEmpty(request.ClientUserAgent) ?
                        HttpContext.Request.Headers["UserAgent"].ToString() : request.ClientUserAgent,
                    SoftwareId = AuthenticatedUser.Software.SoftwareId,
                    UserId = user.UserId
                };
                await UnitOfWork.UserSessions.AddAsync(userSession, cancellationToken);
                await UnitOfWork.CompleteAsync(cancellationToken);
                await _mailService.SendMailAsync(_mailService.EmailConfirmation,
                    new[] { user.Email },
                    Array.Empty<string>(), Array.Empty<string>(),
                    "Humate Email Confirmation", default,
                    user.Username,
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/Email/" +
                    $"EmailConfirmation?user={user.UserId}&secret={WebUtility.UrlEncode(user.EmailConfirmationSecret)}",
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/img/logo.png");
                return Ok(new RegisterAndLoginResponse
                {
                    User = Mapper.Map<Sdk.Model.Identity.User>(user),
                    Token = _jwtAuthenticationService.CreateTokenAuthentication(userSession.UserSessionId)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GenerateJwt/{usernameOrEmail}")]
        public async Task<ActionResult<GenerateJwtResponse>> GenerateJwtAsync(string usernameOrEmail,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var user = Mapper.Map<Sdk.Model.Identity.User>(await UnitOfWork.Users.FindAsync(u =>
                        u.Email == usernameOrEmail || u.Username.ToLower() == usernameOrEmail.ToLower(),
                    cancellationToken,
                    u => u.Language, u => u.Person));
                if (user == null)
                {
                   return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
                }
                var userSession = new UserSession
                {
                    AuthType = AuthType.TrustedUsernameOrEmailProvider,
                    Status = Entity.SSO.Status.Active,
                    Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = HttpContext.Request.Headers["UserAgent"].ToString(),
                    SoftwareId = AuthenticatedUser.Software.SoftwareId,
                    UserId = user.UserId
                };
                await UnitOfWork.UserSessions.AddAsync(userSession, cancellationToken);
                return Ok(new GenerateJwtResponse
                {
                    User = Mapper.Map<Sdk.Model.Identity.User>(user),
                    Token = _jwtAuthenticationService.CreateTokenAuthentication(userSession.UserSessionId)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("SuggestUsername/{suggest}")]
        public async Task<ActionResult<SuggestUsernameResponse>> SuggestUsernameAsync(string suggest,
            CancellationToken cancellationToken = default)
        {
            try
            {
                suggest = suggest.Replace(" ", "");
                while (await UnitOfWork.Users.AnyAsync(u => u.Username == suggest, cancellationToken))
                {
                    suggest += new Random().Next(9);
                }
                return Ok(new SuggestUsernameResponse { Username = suggest });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse>> CreateAsync(
          [FromBody] CreateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = await ValidateForRegisterUserAsync(new RegisterAndLoginRequest
                {
                    Email = request.Email,
                    Username = request.Username,
                    Password = request.Password
                }, cancellationToken);

                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                var software = await UnitOfWork.Softwares.FindAsync(s =>
                     s.SoftwareId == request.SoftwareId, cancellationToken);
                var salt = Cryptography.GenerateSecret();
                var person = new Person
                {
                    SoftwareId = software.SoftwareId,
                    Firstname = request.FirstName,
                    Lastname = request.LastName,
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp(),
                    CountryLivingId = request.CountryLivingId,
                    NationalityId = request.NationalityId
                };

                var user = new User
                {
                    PersonId = person.PersonId,
                    PasswordHash = request.Password.CreateHash(salt),
                    PasswordSalt = salt,
                    Email = request.Email,
                    Mobile = request.Mobile,
                    EmailConfirmed = request.EmailConfirmed,
                    MobileConfirmed = request.MobileConfirmed,
                    TwoFactorEnable = request.TwoFactorEnable,
                    Username = request.Username.Trim().ToLower(),
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp(),
                    RegisterIp = string.IsNullOrEmpty(request.RegisterIp)
                        ? HttpContext.Connection.RemoteIpAddress.ToString() : request.RegisterIp,
                    RegisterDate = DateTime.Now.ConvertToTimestamp(),
                    Status = Entity.Identity.ActiveStatus.Active,
                    Description = request.Description
                };

                var language = await UnitOfWork.Languages.FindAsync(l =>
                    l.LanguageId == request.LanguageId, cancellationToken);
                if (language != null)
                {
                    user.Language = language;
                }

                if (request.RoleId != Guid.Empty)
                {
                    await UnitOfWork.UserRoles.AddAsync(new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = request.RoleId
                    }, cancellationToken);
                }
                else
                {
                    var defaultRole = await UnitOfWork.Roles.FindAsync(r =>
                        r.SoftwareId == request.SoftwareId && r.IsDefault, cancellationToken);
                    if (defaultRole != null)
                    {
                        await UnitOfWork.UserRoles.AddAsync(new UserRole
                        {
                            UserId = user.UserId,
                            RoleId = defaultRole.RoleId
                        }, cancellationToken);
                    }
                }

                await UnitOfWork.Persons.AddAsync(person, cancellationToken);
                await UnitOfWork.Users.AddAsync(user, cancellationToken);
                await UnitOfWork.UserSoftwares.AddAsync(new UserSoftware
                {
                    Software = software,
                    User = user
                }, cancellationToken);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPut("Update")]
        public async Task<ActionResult<BaseResponse>> UpdateAsync(
          [FromBody] UpdateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = await ValidateForUpdateUserAsync(request, cancellationToken);

                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                var user = await UnitOfWork.Users.FindAsync(u => u.UserId == request.UserId,
                    cancellationToken);
                user.Email = request.Email;
                user.Mobile = request.Mobile;
                user.EmailConfirmed = request.EmailConfirmed;
                user.MobileConfirmed = request.MobileConfirmed;
                user.TwoFactorEnable = request.TwoFactorEnable;
                user.Username = request.Username.Trim().ToLower();
                user.UpdateDate = DateTime.Now.ConvertToTimestamp();
                user.LanguageId = request.LanguageId;
                user.Status = (Entity.Identity.ActiveStatus)request.Status;
                user.Description = request.Description;
                if (!string.IsNullOrEmpty(request.Password))
                {
                    var salt = Cryptography.GenerateSecret();
                    user.PasswordHash = request.Password.CreateHash(salt);
                    user.PasswordSalt = salt;
                }
                UnitOfWork.Users.Update(user);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPut("UpdateProfile")]
        public async Task<ActionResult<BaseResponse>> UpdateProfileAsync(
          [FromBody] UpdateProfileRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await UnitOfWork.Users.FindAsync(u => u.UserId == AuthenticatedUser.User.UserId,
                    cancellationToken);
                user.Mobile = request.Mobile;
                user.TwoFactorEnable = request.TwoFactorEnable;
                user.UpdateDate = DateTime.Now.ConvertToTimestamp();
                user.LanguageId = request.LanguageId;
                UnitOfWork.Users.Update(user);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("UploadPicture")]
        public async Task<ActionResult<BaseResponse>> UploadPictureAsync(IFormFile file,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = await ValidateProfilePictureAsync(file, cancellationToken);

                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                var user = await UnitOfWork.Users.FindAsync(u => u.UserId == AuthenticatedUser.User.UserId,
                    cancellationToken);

                var split = file.FileName.Split(".");
                var filename = $"{user.Username}_{DateTime.Now.Ticks}.{split[^1]}";
                user.UpdateDate = DateTime.Now.ConvertToTimestamp();
                user.Picture = filename;
                UnitOfWork.Users.Update(user);
                await UnitOfWork.CompleteAsync(cancellationToken);
                //await _fileStorageService.StoreInUserPathAsync(AuthenticatedUser.User.UserId, filename, file.OpenReadStream(), cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("Picture")]
        public ActionResult<IFormFile> Picture()
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory + Configuration["GlobalParameter:StoragePath"],
                    AuthenticatedUser.User.UserId.ToString());
                var fullPath = $"{path}{Path.DirectorySeparatorChar}{AuthenticatedUser.User.Picture}";
                var picture = _fileStorageService.GetFile(fullPath);
                if (picture == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.UserImageNotFound));
                }

                return new FileStreamResult(picture, new MediaTypeHeaderValue("image/jpeg"));
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpDelete("Delete/{userId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkUserExist = await CheckUserExistAsync(userId, cancellationToken);
                if (checkUserExist != null)
                {
                    return BadRequest(checkUserExist);
                }

                UnitOfWork.UserRoles.Delete(s => s.UserId == userId);
                UnitOfWork.UserSoftwares.Delete(s => s.UserId == userId);
                UnitOfWork.Users.Delete(s => s.UserId == userId);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPatch("EmailConfirmed")]
        public async Task<ActionResult<BaseResponse>> EmailConfirmedAsync([FromBody] EmailConfirmedRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkUserExist = await CheckUserExistAsync(request.UserId, cancellationToken);
                if (checkUserExist != null)
                {
                    return BadRequest(checkUserExist);
                }

                var user = await UnitOfWork.Users.FindAsync(u =>
                    u.UserId == request.UserId && u.EmailConfirmationSecret == request.Secret, cancellationToken);
                if (user == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidDataEntries));
                }

                if (user.EmailConfirmed)
                {
                    return Ok(new BaseResponse());
                }

                if (user.EmailConfirmationExpiry < DateTime.Now.ConvertToTimestamp())
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.ActivationEmailExpired));
                }

                user.EmailConfirmed = true;
                UnitOfWork.Users.Update(user);
                await UnitOfWork.CompleteAsync(cancellationToken);
                await _mailService.SendMailAsync(_mailService.WelcomeUser,
                    new[] { user.Email },
                    Array.Empty<string>(), Array.Empty<string>(),
                    "Humate Welcome", default,
                    user.Email,
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/img/logo.png");
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("SendEmailConfirmation")]
        public async Task<ActionResult<BaseResponse>> SendEmailConfirmationAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (AuthenticatedUser.User.EmailConfirmed)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.EmailConfirmedBefore));
                }

                var user = AuthenticatedUser.User;
                user.EmailConfirmationExpiry = DateTime.Now.AddDays(3).ConvertToTimestamp();
                user.EmailConfirmationSecret = Cryptography.GenerateSecret(32);
                UnitOfWork.Users.Update(user);
                await UnitOfWork.CompleteAsync(cancellationToken);
                await _mailService.SendMailAsync(_mailService.EmailConfirmation,
                    new[] { AuthenticatedUser.User.Email },
                    Array.Empty<string>(), Array.Empty<string>(),
                    "Humate Email Confirmation", default,
                    user.Username,
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/Email/" +
                    $"EmailConfirmation?user={user.UserId}&secret={WebUtility.UrlEncode(user.EmailConfirmationSecret)}",
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/img/logo.png");
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        #region Methods

        [NonAction]
        private async Task<BaseResponse> ValidateProfilePictureAsync(IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (file == null)
            {
                return BaseResponseCollection.GetBaseResponse(
                    RequestResult.ProfilePictureNull);
            }

            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream, cancellationToken);
                if (memoryStream.Length > 2097152)
                {
                    return BaseResponseCollection.GetBaseResponse(
                        RequestResult.ProfilePictureSize);
                }
            }

            if (!ImageHelper.IsImageFile(file))
            {
                return BaseResponseCollection.GetBaseResponse(
                    RequestResult.InvalidFileFormat);
            }

            return null;
        }

        [NonAction]
        private async Task<BaseResponse> ValidateForRegisterUserAsync(RegisterUserRequest request,
            CancellationToken cancellationToken = default)
        {
            if (await UnitOfWork.Users.AnyAsNoTrackingAsync(u =>
                u.Username == request.Username.Trim().ToLower(), cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(
                    RequestResult.UsernameExist);
            }
            if (await UnitOfWork.Users.AnyAsNoTrackingAsync(u =>
                u.Email == request.Email /*&& u.EmailConfirmed*/, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(
                    RequestResult.EmailExist);
            }
            if (request.Password.Length < 8)
            {
                return BaseResponseCollection.GetBaseResponse(
                    RequestResult.LowPasswordLength);
            }
            return null;
        }

        [NonAction]
        private async Task<BaseResponse> ValidateForUpdateUserAsync(UpdateRequest request,
                   CancellationToken cancellationToken = default)
        {
            if (await UnitOfWork.Users.AnyAsNoTrackingAsync(u =>
                u.Username == request.Username.Trim().ToLower() && u.UserId != request.UserId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(
                    RequestResult.UsernameExist);
            }
            if (await UnitOfWork.Users.AnyAsNoTrackingAsync(u =>
                u.Email == request.Email && u.EmailConfirmed && u.UserId != request.UserId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(
                    RequestResult.EmailExist);
            }
            return null;
        }
        #endregion
    }
}