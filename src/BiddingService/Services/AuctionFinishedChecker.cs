using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService;

public class AuctionFinishedChecker : BackgroundService
{
    readonly ILogger<AuctionFinishedChecker> _logger;
    readonly IServiceProvider _service;
    public AuctionFinishedChecker(ILogger<AuctionFinishedChecker> logger, IServiceProvider service)
    {
        _logger = logger;
        _service = service;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting checking Auction Activation");

        stoppingToken.Register(() => _logger.LogInformation("Auction checker stopping"));

        while(!stoppingToken.IsCancellationRequested)
        {
            await CheckAuction(stoppingToken);

            await Task.Delay(5000, stoppingToken);
        }

        
    }
    private async Task CheckAuction(CancellationToken stoppingToken)
    {
        var finishedAuction =await DB.Find<Auction>()
            .Match(x => x.AuctionEnd <= DateTime.UtcNow)
            .Match(x => !x.Finished)
            .ExecuteAsync(stoppingToken);
        if (finishedAuction.Count == 0) return; 

        _logger.LogInformation("Auction found {count}", finishedAuction.Count);

        using var scope = _service.CreateScope();
        var endPoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        foreach(var auction in finishedAuction)
        {
            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);

            var winningBid = await DB.Find<Bid>()
                .Match(x =>  x.AuctionId == auction.ID)
                .Match(x => x.BidStatus == BidStatus.Accepted)
                .Sort(s => s.Descending(x => x.Amount))
                .ExecuteFirstAsync(stoppingToken);

            await endPoint.Publish(new AuctionFinished
            {
                ItemSold = winningBid != null,  
                AuctionId = auction.ID,
                Winner = winningBid?.Bidder,
                Amount = winningBid?.Amount,
                Seller = auction.Seller
            }, stoppingToken);
           
        }
    }
}
