using Microsoft.EntityFrameworkCore;

namespace CustomOidcClaims.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> opts)
            : base(opts)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}