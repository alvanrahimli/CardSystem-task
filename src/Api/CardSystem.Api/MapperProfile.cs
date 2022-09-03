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
        CreateMap<CardMessage, Card>()
            .ForMember(b => b.Id, x => x.Ignore())
            .ForMember(b => b.State, x => x.MapFrom(a => Enum.Parse<CardState>(a.State)))
            .ForMember(b => b.Type, x => x.MapFrom(a => Enum.Parse<CardState>(a.Type)))
            .ForMember(b => b.Currency, x => x.MapFrom(a => a.Currency == null ? (Currency?)null : Enum.Parse<Currency>(a.Currency)));

        CreateMap<Account, AccountMessage>()
            .ForMember(b => b.Type, x => x.MapFrom(a => a.Type.ToString()));

        CreateMap<Transaction, TransactionMessage>()
            .ForMember(b => b.Type, x => x.MapFrom(a => a.Type));
        
        CreateMap<Vendor, VendorMessage>();

        CreateMap<AppUser, ProfileMessage>();
    }
}