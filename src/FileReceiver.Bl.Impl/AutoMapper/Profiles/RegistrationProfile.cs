using AutoMapper;

using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Bl.Impl.AutoMapper.Profiles
{
    public class RegistrationProfile : Profile
    {
        public RegistrationProfile()
        {
            CreateMap<UserModel, UserEntity>().ReverseMap();
        }
    }
}
