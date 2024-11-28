using Finance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Finance.Database
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<FinanceMonth> FinanceMonths { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seeding data for Months
            modelBuilder.Entity<Month>().HasData(
                new Month { Id = 1, Name = "January", MonthOrdinal = 1 },
                new Month { Id = 2, Name = "February", MonthOrdinal = 2 },
                new Month { Id = 3, Name = "March", MonthOrdinal = 3 },
                new Month { Id = 4, Name = "April", MonthOrdinal = 4 },
                new Month { Id = 5, Name = "May", MonthOrdinal = 5 },
                new Month { Id = 6, Name = "June", MonthOrdinal = 6 },
                new Month { Id = 7, Name = "July", MonthOrdinal = 7 },
                new Month { Id = 8, Name = "August", MonthOrdinal = 8 },
                new Month { Id = 9, Name = "September", MonthOrdinal = 9 },
                new Month { Id = 10, Name = "October", MonthOrdinal = 10 },
                new Month { Id = 11, Name = "November", MonthOrdinal = 11 },
                new Month { Id = 12, Name = "December", MonthOrdinal = 12 }
            );

            // Configure the relationship between FinanceMonth and Month
            modelBuilder.Entity<FinanceMonth>()
                .HasOne(fm => fm.Month) // Navigation property
                .WithMany()             // No inverse navigation in Month
                .HasForeignKey(fm => fm.MonthId) // Foreign Key
                .OnDelete(DeleteBehavior.Restrict); // Optional: Configure delete behavior


            // Configure the Transaction relationship with FinanceMonth
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.FinanceMonth)       // Navigation property
                .WithMany()                        // No inverse navigation in FinanceMonth
                .HasForeignKey(t => t.FinanceMonthId) // Foreign Key
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            // Configure the Transaction relationship with Category
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)           // Navigation property
                .WithMany()                        // No inverse navigation in Category
                .HasForeignKey(t => t.CategoryId)  // Foreign Key
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            base.OnModelCreating(modelBuilder);
        }
    }
}