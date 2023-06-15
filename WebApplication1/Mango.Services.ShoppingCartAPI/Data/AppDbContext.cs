using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        

        public DbSet<CartHeader> Cartheaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

    }
}
