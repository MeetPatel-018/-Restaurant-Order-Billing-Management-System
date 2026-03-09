using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Bill> Bills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            // Configure MenuItem
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.ItemID);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasPrecision(10, 2);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Available");
                
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.MenuItems)
                      .HasForeignKey(e => e.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Table
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.TableID);
                entity.Property(e => e.TableNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Available");
                entity.HasIndex(e => e.TableNumber).IsUnique();
            });

            // Configure Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderID);
                entity.Property(e => e.OrderStatus).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.TotalAmount).HasPrecision(10, 2).HasDefaultValue(0);
                
                entity.HasOne(e => e.Table)
                      .WithMany(t => t.Orders)
                      .HasForeignKey(e => e.TableID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemID);
                entity.Property(e => e.Price).HasPrecision(10, 2);
                entity.Property(e => e.SubTotal).HasPrecision(10, 2);
                
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(e => e.MenuItem)
                      .WithMany(mi => mi.OrderItems)
                      .HasForeignKey(e => e.ItemID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Bill
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.BillID);
                entity.Property(e => e.SubTotal).HasPrecision(10, 2);
                entity.Property(e => e.Tax).HasPrecision(10, 2);
                entity.Property(e => e.FinalAmount).HasPrecision(10, 2);
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50).HasDefaultValue("Cash");
                
                entity.HasOne(e => e.Order)
                      .WithOne(o => o.Bill)
                      .HasForeignKey<Bill>(e => e.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, CategoryName = "Appetizers" },
                new Category { CategoryID = 2, CategoryName = "Main Course" },
                new Category { CategoryID = 3, CategoryName = "Desserts" },
                new Category { CategoryID = 4, CategoryName = "Beverages" },
                new Category { CategoryID = 5, CategoryName = "Soups" }
            );

            // Seed Menu Items
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { ItemID = 1, ItemName = "Spring Rolls", CategoryID = 1, Price = 5.99m, Description = "Crispy vegetable spring rolls", Status = "Available" },
                new MenuItem { ItemID = 2, ItemName = "Chicken Soup", CategoryID = 5, Price = 4.99m, Description = "Hot and sour chicken soup", Status = "Available" },
                new MenuItem { ItemID = 3, ItemName = "Grilled Chicken", CategoryID = 2, Price = 12.99m, Description = "Tender grilled chicken with herbs", Status = "Available" },
                new MenuItem { ItemID = 4, ItemName = "Pasta Carbonara", CategoryID = 2, Price = 10.99m, Description = "Classic Italian pasta with bacon", Status = "Available" },
                new MenuItem { ItemID = 5, ItemName = "Ice Cream", CategoryID = 3, Price = 3.99m, Description = "Vanilla ice cream with chocolate sauce", Status = "Available" },
                new MenuItem { ItemID = 6, ItemName = "Coffee", CategoryID = 4, Price = 2.99m, Description = "Freshly brewed coffee", Status = "Available" },
                new MenuItem { ItemID = 7, ItemName = "Lemonade", CategoryID = 4, Price = 2.49m, Description = "Fresh lemonade", Status = "Available" }
            );

            // Seed Tables
            modelBuilder.Entity<Table>().HasData(
                new Table { TableID = 1, TableNumber = "T1", Capacity = 4, Status = "Available" },
                new Table { TableID = 2, TableNumber = "T2", Capacity = 4, Status = "Available" },
                new Table { TableID = 3, TableNumber = "T3", Capacity = 2, Status = "Available" },
                new Table { TableID = 4, TableNumber = "T4", Capacity = 6, Status = "Available" },
                new Table { TableID = 5, TableNumber = "T5", Capacity = 8, Status = "Available" },
                new Table { TableID = 6, TableNumber = "T6", Capacity = 4, Status = "Available" }
            );
        }
    }
}
