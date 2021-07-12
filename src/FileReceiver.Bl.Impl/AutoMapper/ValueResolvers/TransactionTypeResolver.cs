using AutoMapper;

using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Bl.Impl.AutoMapper.ValueResolvers
{
    public class TransactionTypeResolver : IValueResolver<TransactionEntity, TransactionModel, TransactionDataModel>
    {
        public TransactionDataModel Resolve(TransactionEntity source, TransactionModel destination, TransactionDataModel destMember,
            ResolutionContext context)
        {
            return new TransactionDataModel(source.TransactionData);
        }
    }
}
