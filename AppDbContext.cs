using blackbird_crm.Models;
using Microsoft.EntityFrameworkCore;

namespace blackbird_crm.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }    
        public DbSet<Transaction> Transactions { get; set; }       
    }
}