using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.Identity.Response.UserRole;


namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    public class UserRoleDomainProfile : Profile
    {
        public UserRoleDomainProfile()
        {
            CreateMap<UserRole, AssignmentUserRole>()
                .ForMember(des => des.UserId,
                    op => op.MapFrom(src => src.UserId))
                .ForMember(des => des.Username,
                    op => op.MapFrom(src => src.User.Username))
                .ForMember(des => des.Firstname,
                    op => op.MapFrom(src => src.User.Firstname))
                .ForMember(des => des.Lastname,
                    op => op.MapFrom(src => src.User.Lastname))
                .ForMember(des => des.Email,
                    op => op.MapFrom(src => src.User.Email))
                .ForMember(des => des.Assigned,
                    op => op.MapFrom(src => src.Role != null))
                .ForMember(des => des.Status,
                    op => op.MapFrom(src =>
                        src.Role != null ? "Assigned" : "Not Assign"))
                .ForMember(des => des.AssignmentDate,
                    op => op.MapFrom(src => src.CreateDate));

        }
    }
}
