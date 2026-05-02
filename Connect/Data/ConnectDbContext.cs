using Connect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Connect.Data
{
    public class ConnectDbContext : IdentityDbContext<AppUser>
    {
        public ConnectDbContext(DbContextOptions<ConnectDbContext> options) : base(options)
        {

        }

        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ConnectDbContext).Assembly);
        }
    }
}
