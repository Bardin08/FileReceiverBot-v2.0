using AutoMapper;

using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Bl.Impl.AutoMapper.ValueResolvers
{
    public class FileReceiverSessionTypeResolver
        : IValueResolver<FileReceivingSessionEntity, FileReceivingSessionModel, ConstraintsModel>
    {
        public ConstraintsModel Resolve(FileReceivingSessionEntity source, FileReceivingSessionModel destination,
            ConstraintsModel destMember, ResolutionContext context)
        {
            return new ConstraintsModel(source.Constrains);
        }
    }
}
