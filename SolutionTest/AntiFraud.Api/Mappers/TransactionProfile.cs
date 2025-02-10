using AntiFraud.Api.Endpoints;
using AntiFraud.Core.Entities;
using AntiFraud.Core.Enums;
using AutoMapper;

namespace AntiFraud.Api.Mappers
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {            
            CreateMap<CreateTransactionRequest, Transaction>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TransactionStatus.Pending))
                .ForMember(dest => dest.TransactionExternalId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.SourceAccountId, opt => opt.MapFrom(src => src.bodyTransaction.SourceAccountId))
                .ForMember(dest => dest.TargetAccountId, opt => opt.MapFrom(src => src.bodyTransaction.TargetAccountId))
                .ForMember(dest => dest.TransferTypeId, opt => opt.MapFrom(src => src.bodyTransaction.TransferTypeId))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.bodyTransaction.Value));
        }
    }
}
