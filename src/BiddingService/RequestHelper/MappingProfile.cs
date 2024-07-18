using AutoMapper;
using Contracts;

namespace BiddingService;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Bid, BidDTO>();
        CreateMap<Bid, BidPlaced>();
    }
}
