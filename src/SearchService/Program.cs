using System.Net;
using MassTransit; 
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionDeteletedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionFinishedConsumer>();
    x.AddConsumersFromNamespaceContaining<BidPlacedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false)); //menambahkan di prefix karakter search dan memberi ('-')

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(
                builder.Configuration["RabbitMq:Host"],
                "/",
                host =>
                {
                    host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                    host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                }
            );
        
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));

            e.ConfigureConsumer<AuctionCreatedConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("search-auction-deleted", e =>
        {
            e.UseMessageRetry(r => r.Interval(5,5));

            e.ConfigureConsumer<AuctionDeteletedConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("search-auction-updated", e =>
        {
            e.UseMessageRetry(r => r.Interval(5,5));

            e.ConfigureConsumer<AuctionUpdatedConsumer>(ctx);
        });
        cfg.ConfigureEndpoints(ctx);
    });
});


var app = builder.Build();  

// await DB.InitAsync("SearchDB", MongoClientSettings
//             .FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection")));

// await DB.Index<Item>()
//     .Key(x => x.Make, KeyType.Text)
//     .Key(x => x.Model, KeyType.Text)
//     .Key(x => x.Color, KeyType.Text)
//     .CreateAsync();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async() =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }catch(Exception e)
    {
        Console.WriteLine(e);
    }
});


app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
 => HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));