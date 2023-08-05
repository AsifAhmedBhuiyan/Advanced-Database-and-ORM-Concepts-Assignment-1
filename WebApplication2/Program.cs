using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Data;
using WebApplication2.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("WebApplication2ContextConnection");

builder.Services.AddDbContext<WebApplication2Context>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<WebApplication2Context>();
    SeedData.Initialize(dbContext);
}


// An endpoint that shows all of the laptops available in a store
// with stores/{storeNumber}/inventory. Laptops with 0 or less
// quantity should not be shown.

app.MapGet("/store/{storeNumber}/inventory", async (HttpContext httpContext, string storeNumber) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<WebApplication2Context>();

        try
        {
            if (!Guid.TryParse(storeNumber, out Guid parsedStoreNumber))
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new { Message = "Invalid store number format." });
                return;
            }

            var store = await dbContext.Store.FindAsync(parsedStoreNumber);

            if (store == null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsJsonAsync(new { Message = "Store not found." });
                return;
            }

            var laptopsInStore = new List<Laptop>();
            var allLaptops = await dbContext.Laptops.Where(laptop => laptop.StoreNumber == parsedStoreNumber).ToListAsync();

            foreach (var laptop in allLaptops)
            {
                if (laptop.InStockQuantity > 0)
                {
                    laptopsInStore.Add(laptop);
                }
            }

            if (laptopsInStore.Count == 0)
            {
                await httpContext.Response.WriteAsJsonAsync(new { Message = "No laptops available in the store." });
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync(laptopsInStore);
            }
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new { Message = "An error occurred while processing the request.", Error = ex.Message });
        }
    }
});



// An endpoint for posting a new Quantity for a Laptop at a
// specific store in stores/{storeNumber}/{laptopNumber}/changeQuantity?amount

app.MapPost("/store/{storeNumber}/{laptopNumber}/changeQuantity", async (HttpContext httpContext, string storeNumber, Guid laptopNumber) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<WebApplication2Context>();

        if (!Guid.TryParse(storeNumber, out Guid parsedStoreNumber))
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new { Message = "Invalid store number format." });
            return;
        }

        var store = await dbContext.Store.FindAsync(parsedStoreNumber);

        if (store == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(new { Message = "Store not found." });
            return;
        }

        var laptopsToUpdate = httpContext.Request.Query["laptopNumber"].ToString().Split(',');
        var updateResults = new List<string>();

        foreach (var laptopId in laptopsToUpdate)
        {
            if (!Guid.TryParse(laptopId, out Guid parsedLaptopNumber))
            {
                updateResults.Add($"Laptop with ID '{laptopId}' has an invalid format.");
                continue;
            }

            var laptop = await dbContext.Laptops.FindAsync(parsedLaptopNumber);

            if (laptop == null)
            {
                updateResults.Add($"Laptop with ID '{parsedLaptopNumber}' was not found.");
                continue;
            }

            if (!int.TryParse(httpContext.Request.Query["amount"], out int newQuantity))
            {
                updateResults.Add($"Invalid quantity format for laptop with ID '{parsedLaptopNumber}'.");
                continue;
            }

            laptop.InStockQuantity = newQuantity;
            dbContext.Laptops.Update(laptop);
            updateResults.Add($"Laptop with ID '{parsedLaptopNumber}' quantity was updated successfully.");
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new { Message = "An error occurred while saving the changes.", Error = ex.Message });
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(new { UpdateResults = updateResults });
    }
});


// An endpoint for getting the average price of all
// laptops among a specific brand, returned as
// { LaptopCount: [value], AveragePrice: [value]}


app.MapGet("/brands/{brandId}/averagePrice", async (HttpContext httpContext, int brandId) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<WebApplication2Context>();

        var brand = await dbContext.Brands.FindAsync(brandId);

        if (brand == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(new { Message = "Brand not found." });
            return;
        }

        var laptopsWithBrand = await dbContext.Laptops.Where(laptop => laptop.BrandId == brandId).ToListAsync();

        if (laptopsWithBrand.Count == 0)
        {
            await httpContext.Response.WriteAsJsonAsync(new { Message = "No laptops found for the specified brand." });
            return;
        }

        var laptopCount = laptopsWithBrand.Count;
        decimal totalPrice = 0;

        try
        {
            for (int i = 0; i < laptopCount; i++)
            {
                totalPrice += laptopsWithBrand[i].Price;
            }

            var averagePrice = totalPrice / laptopCount;

            await httpContext.Response.WriteAsJsonAsync(new { LaptopCount = laptopCount, AveragePrice = averagePrice });
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new { Message = "An error occurred while calculating the average price.", Error = ex.Message });
            return;
        }
    }
});


// An endpoint which dynamically groups and returns all Stores
// according to the Province in which they are in. This endpoint
// should not display any data from any model other than the
// Stores queried, and should only display provinces that have
// stores in them.

app.MapGet("/api/store-by-province", async (HttpContext httpContext) =>
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<WebApplication2Context>();

            string provinceName = httpContext.Request.Query["province"].ToString();

            if (Enum.TryParse(provinceName, ignoreCase: true, out CanadianProvince provinceEnum))
            {
                var allStores = await dbContext.Store.ToListAsync();

                var storesByProvince = new List<object>();

                foreach (var store in allStores)
                {
                    if (store.Province == provinceEnum)
                    {
                        storesByProvince.Add(new
                        {
                            Province = store.Province.ToString(),
                            Store = store
                        });
                    }
                }

                if (storesByProvince.Count == 0)
                {
                    await httpContext.Response.WriteAsJsonAsync(new { Message = "No stores found." });
                }
                else
                {
                    await httpContext.Response.WriteAsJsonAsync(storesByProvince);
                }
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync(new { Message = "Invalid province name." });
            }
        }
    }
    catch (Exception ex)
    {
        
        await httpContext.Response.WriteAsJsonAsync(new { Message = "An error occurred while processing your request.", Error = ex.Message });
    }
});




app.Run();
