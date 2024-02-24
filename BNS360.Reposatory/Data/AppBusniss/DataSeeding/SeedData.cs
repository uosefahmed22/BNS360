using BNS360.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BNS360.Reposatory.Data.AppBusniss.DataSeeding;

public class SeedData
{
    public static async Task Seed(AppBusnissDbContext context)
    {
        if (!context.Categories.Any())
        {
            var data = File.ReadAllText("../BNS360.Reposatory/Data/AppBusniss/DataSeeding/Categories.json");
            var categories = JsonSerializer.Deserialize<List<Category>>(data);
            if (categories is not null)
            {
                await context.AddRangeAsync(categories);
            }
            await context.SaveChangesAsync();
        }
        if (context.Busnisses.Count() == 1)
        {
            var categoryIds = await context.Categories.Select(c => c.Id).ToListAsync();
              
            var busniss = new Busniss() { AboutAR = "nice place", NameAR = "kfc", CategoryId = categoryIds[0], UserId = Guid.NewGuid() };

            busniss.Reviews = new List<Review>(){
                new Review() { BusnissId = busniss.Id, Rate = 3, Comment = "greate place" },
                new Review() { BusnissId = busniss.Id, Rate = 4, Comment = "greate place" },
                new Review() { BusnissId = busniss.Id, Rate = 5, Comment = "greate place" },
                new Review() { BusnissId = busniss.Id, Rate = 3, Comment = "greate place" }
            };
            busniss.Location = new()
            {
                Latitude = 60.14m,
                Longitude = 35.25m,
                Address = "this streat"
            };
            var busniss2 = new Busniss() { AboutAR = "nice place", NameAR = "kfc", CategoryId = categoryIds[1], UserId = Guid.NewGuid() };
            busniss2.Reviews = new List<Review>(){
                new Review() { BusnissId = busniss2.Id, Rate = 3, Comment = "greate place" },
                new Review() { BusnissId = busniss2.Id, Rate = 4, Comment = "greate place" },
                new Review() { BusnissId = busniss2.Id, Rate = 3, Comment = "greate place" }
            };
            busniss2.Location = new()
            {
                Latitude = 60.14m,
                Longitude = 35.25m,
                Address = "this streat"
            };
            var busniss3 = new Busniss() { AboutAR = "nice place", NameAR = "kfc", CategoryId = categoryIds[1], UserId = Guid.NewGuid() };
            busniss3.Location = new()
            {
                Latitude = 60.14m,
                Longitude = 35.25m,
                Address = "this streat"
            };

            await context.AddAsync(busniss);
            await context.AddAsync(busniss2);
            await context.AddAsync(busniss3);   

            
        }
        await context.SaveChangesAsync();   
 
    }
}
