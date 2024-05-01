using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionDeteletedConsumer : IConsumer<AuctionDeleted>
{
   
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine("consumee");
        var result =  await DB.DeleteAsync<Item>(context.Message.Id);

        if(!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionDeleted),"Cannot delete item in Mongo DB");
        }
    }
}
