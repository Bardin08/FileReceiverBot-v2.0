using AutoMapper;

using FileReceiver.Bl.Impl.AutoMapper.ValueResolvers;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Bl.Impl.AutoMapper.Profiles
{
    public class TransactionsProfile : Profile
    {
        public TransactionsProfile()
        {
            CreateMap<TransactionModel, TransactionEntity>()
                .ForMember(x => x.TransactionData,
                    opt => opt.MapFrom(x => x.TransactionData.ParametersAsJson));
            CreateMap<TransactionEntity, TransactionModel>()
                .ForMember(x => x.TransactionData,
                    opt => opt.MapFrom(new TransactionTypeResolver()));
        }
    }
}
