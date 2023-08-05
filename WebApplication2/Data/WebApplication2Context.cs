using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class WebApplication2Context : DbContext
    {
        public WebApplication2Context(DbContextOptions options) : base(options) { }

        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Laptop> Laptops { get; set; } = null!;
        public DbSet<Store> Store { get; set; } = null!;
    }
}
