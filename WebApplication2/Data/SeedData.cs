using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;


namespace WebApplication2.Data
{
    public static class SeedData
    {
        public static async Task Initialize(WebApplication2Context context)
        {

            await context.Database.EnsureCreatedAsync();

            if (!context.Laptops.Any())
            {
                SeedLaptops(context);
            }

            if (!context.Brands.Any())
            {
                SeedBrands(context);
            }

            if (!context.Store.Any())
            {
                SeedStores(context);
            }
        }

        private static void SeedLaptops(WebApplication2Context context)
        {
            var laptops = new List<Laptop>
            {
                new Laptop { Model = "Laptop A", Price = 1500, Condition = LaptopCondition.New, InStockQuantity = 8 },
                new Laptop { Model = "Laptop B", Price = 1300, Condition = LaptopCondition.Refurbished, InStockQuantity = 12 },
                new Laptop { Model = "Laptop C", Price = 600, Condition = LaptopCondition.New, InStockQuantity = 11 }
            };

            context.Laptops.AddRange(laptops);

        }

        private static void SeedBrands(WebApplication2Context context)
        {
            var brands = new List<Brand>
            {
                new Brand { Name = "Brand X" },
                new Brand { Name = "Brand Y" },
                new Brand { Name = "Brand Z" }
            };

            context.Brands.AddRange(brands);

        }

        private static void SeedStores(WebApplication2Context context)
        {
            var store = new List<Store>
            {
                new Store { StoreNumber = Guid.NewGuid(), StreetNameAndNumber = "123 Ontario St", Province = CanadianProvince.Ontario },
                new Store { StoreNumber = Guid.NewGuid(), StreetNameAndNumber = "123 BC St", Province = CanadianProvince.BritishColumbia },
                new Store { StoreNumber = Guid.NewGuid(), StreetNameAndNumber = "123 Calgary St", Province = CanadianProvince.Alberta }
             };
            context.Store.AddRange(store);
            context.SaveChanges();
        }

    }
}
