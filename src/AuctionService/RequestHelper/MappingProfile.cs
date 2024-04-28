using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Enum;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelper;

public class MappingProfile :Profile
{
    public MappingProfile(){
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>().ForMember(d=>d.Item, o=>o.MapFrom(s=>s));
        CreateMap<CreateAuctionDto, Item>();
        CreateMap<AuctionDto, AuctionCreated>(); 
    }
}
