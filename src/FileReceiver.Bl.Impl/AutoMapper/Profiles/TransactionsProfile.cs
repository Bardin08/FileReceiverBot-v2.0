using AutoMapper;

using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Bl.Impl.AutoMapper.Profiles
{
    public class TransactionsProfile : Profile
    {
        public TransactionsProfile()
        {
            CreateMap<TransactionModel, TransactionEntity>().ReverseMap();
        }
    }
}
