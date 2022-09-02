using AutoMapper;
using CardSystem.Api.Messages;
using CardSystem.Domain.Models;

namespace CardSystem.Api;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Card, CardMessage>()
            .ForMember(b => b.State, x => x.MapFrom(a => a.State.ToString()))
            .ForMember(b => b.Type, x => x.MapFrom(a => a.Type.ToString()))
            .ForMember(b => b.Currency, x => x.MapFrom(a => a.Currency.ToString()));

        CreateMap<Account, AccountMessage>()
            .ForMember(b => b.Type, x => x.MapFrom(a => a.Type.ToString()));

        CreateMap<Transaction, TransactionMessage>()
            .ForMember(b => b.Type, x => x.MapFrom(a => a.Type));
        
        CreateMap<Vendor, VendorMessage>();
    }
}