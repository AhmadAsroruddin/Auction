using Contracts;
using MassTransit;

namespace SearchService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    public Task Consume(ConsumeContext<AuctionCreated> context)
    {
        throw new NotImplementedException();
    }
}
