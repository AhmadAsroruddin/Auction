﻿using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedConsumer(IMapper mapper){
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        var item = _mapper.Map<Item>(context.Message);

        var result = await DB.Update<Item>()
            .Match(x => x.ID == context.Message.Id.ToString())
            .ModifyOnly(x => new
            {
                x.Color,
                x.Model,
                x.Make,
                x.Year,
                x.Mileage
            }, item)
            .ExecuteAsync();

        if(!result.IsAcknowledged){
            throw new MessageException(typeof(AuctionUpdated), "There is problem updating on Mongo DB");
        }
    }
}
