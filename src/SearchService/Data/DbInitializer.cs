using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchService;

public class DbInitializer
{   
    public static async Task InitDb(WebApplication webApp)
    {
        await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(webApp.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text)
        .CreateAsync();

        var count = await DB.CountAsync<Item>();

        using var scope = webApp.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await httpClient.GetItemForSearchDb();

        Console.WriteLine(items.Count + " returner from database");

        if(items.Count > 0) await DB.SaveAsync(items);
    }
}
