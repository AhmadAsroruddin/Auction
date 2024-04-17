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

        if(count == 0)
        {
            Console.WriteLine("No data to seed" );
            var itemData = await File.ReadAllTextAsync("Data/auctions.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            await DB.SaveAsync(items);
        }
    }
}
