using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WasHere.Database
{

    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ActivationKey { get; set; }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Specify the MySQL server version explicitly
                optionsBuilder.UseMySql("server=45.88.108.162;port=3306;database=db_240335;user=db_240335;password=WLzrpDowbQVxm5BhEdW0",
                    new MySqlServerVersion(new Version(8, 0, 28))); // Replace with your MySQL server version
            }
        }

        //Scaffold-DbContext "Server=45.88.108.162;Port=3306;Database=db_240335;Uid=db_240335;Pwd=WLzrpDowbQVxm5BhEdW0;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models


        public async Task AddUserAsync(User user)
        {
            await Users.AddAsync(user);
            await SaveChangesAsync();
        }
    }
}
