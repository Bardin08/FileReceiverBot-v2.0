using AutoMapper;

using FileReceiver.Bl.Impl.AutoMapper.ValueResolvers;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Bl.Impl.AutoMapper.Profiles
{
    public class FileReceivingSessionProfile : Profile
    {
        public FileReceivingSessionProfile()
        {
            CreateMap<FileReceivingSessionModel, FileReceivingSessionEntity>()
                .ForMember(x => x.Constrains,
                    opt => opt.MapFrom(x => x.Constrains.ConstraintsAsJson));
            CreateMap<FileReceivingSessionEntity, FileReceivingSessionModel>()
                .ForMember(x => x.Constrains,
                    opt => opt.MapFrom(new FileReceiverSessionTypeResolver()));
        }
    }
}
