using AutoMapper;

namespace BiddingService;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Bid, BidDTO>();
    }
}
