using Microsoft.EntityFrameworkCore;
using ShoppingCartCustomerMVC.Models.Entities;

namespace ShoppingCartCustomerMVC.Data
{
    public class ShoppingCartDbContext : DbContext
    {
        public ShoppingCartDbContext(
            DbContextOptions<ShoppingCartDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Cart> Cart { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");

                entity.HasKey(c => c.CustomerID);

                entity.Property(c => c.CustomerName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(c => c.Email)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(c => c.Phone)
                    .HasMaxLength(15);

                entity.Property(c => c.City)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(p => p.ProductID);

                entity.Property(p => p.ProductName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.Price)
                    .HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.HasKey(c => c.CartID);

                entity.HasIndex(c => c.CustomerID)
                    .IsUnique();

                entity.HasOne(c => c.Customer)
                    .WithOne(customer => customer.Cart)
                    .HasForeignKey<Cart>(c => c.CustomerID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItems");

                entity.HasKey(ci => ci.CartItemID);

                entity.HasOne(ci => ci.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(ci => ci.CartID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                    .WithMany()
                    .HasForeignKey(ci => ci.ProductID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ci => new
                {
                    ci.CartID,
                    ci.ProductID
                })
                .IsUnique();
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("Wishlist");

                entity.HasKey(w => w.WishlistID);

                entity.Property(w => w.AddedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(w => w.Customer)
                    .WithMany()
                    .HasForeignKey(w => w.CustomerID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.Product)
                    .WithMany()
                    .HasForeignKey(w => w.ProductID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(w => new
                {
                    w.CustomerID,
                    w.ProductID
                })
                .IsUnique();
            });
        }
    }
}