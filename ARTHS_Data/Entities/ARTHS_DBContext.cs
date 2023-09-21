using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ARTHS_Data.Entities
{
    public partial class ARTHS_DBContext : DbContext
    {
        public ARTHS_DBContext()
        {
        }

        public ARTHS_DBContext(DbContextOptions<ARTHS_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountRole> AccountRoles { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<CartItem> CartItems { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<FeedbackProduct> FeedbackProducts { get; set; } = null!;
        public virtual DbSet<FeedbackStaff> FeedbackStaffs { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<OwnerAccount> OwnerAccounts { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductOrder> ProductOrders { get; set; } = null!;
        public virtual DbSet<ProductOrderDetail> ProductOrderDetails { get; set; } = null!;
        public virtual DbSet<ProductPrice> ProductPrices { get; set; } = null!;
        public virtual DbSet<RepairOrder> RepairOrders { get; set; } = null!;
        public virtual DbSet<RepairOrderDetail> RepairOrderDetails { get; set; } = null!;
        public virtual DbSet<RepairService> RepairServices { get; set; } = null!;
        public virtual DbSet<StaffAccount> StaffAccounts { get; set; } = null!;
        public virtual DbSet<Vehicle> Vehicles { get; set; } = null!;
        public virtual DbSet<Warranty> Warranties { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=TAN-TRUNG\\HAMMER;Database=ARTHS_DB;Persist Security Info=False;User ID=sa;Password=123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.PhoneNumber, "UQ__Account__85FB4E380A417AB5")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Account__RoleId__276EDEB3");
            });

            modelBuilder.Entity<AccountRole>(entity =>
            {
                entity.ToTable("AccountRole");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.HasIndex(e => e.RepairOrderId, "UQ__Bill__016C098F34C9406F")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BillDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.HasOne(d => d.RepairOrder)
                    .WithOne(p => p.Bill)
                    .HasForeignKey<Bill>(d => d.RepairOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bill__RepairOrde__00200768");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.HasIndex(e => e.CustomerId, "UQ__Cart__A4AE64D94A54E4B8")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.Cart)
                    .HasForeignKey<Cart>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cart__CustomerId__6383C8BA");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => new { e.CartId, e.ProductId })
                    .HasName("PK__CartItem__9AFC1BDBA1C9CDB8");

                entity.ToTable("CartItem");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CartItem__CartId__66603565");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CartItem__Produc__6754599E");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CategoryName).HasMaxLength(100);
            });

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.ToTable("CustomerAccount");

                entity.HasIndex(e => e.AccountId, "UQ__Customer__349DA5A7CF7E5130")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.CustomerAccount)
                    .HasForeignKey<CustomerAccount>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerA__Accou__33D4B598");
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("Discount");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.Title).HasMaxLength(255);
            });

            modelBuilder.Entity<FeedbackProduct>(entity =>
            {
                entity.ToTable("FeedbackProduct");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.FeedbackProducts)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__FeedbackP__Custo__5DCAEF64");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.FeedbackProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FeedbackP__Produ__5EBF139D");
            });

            modelBuilder.Entity<FeedbackStaff>(entity =>
            {
                entity.ToTable("FeedbackStaff");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.SendDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.FeedbackStaffs)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__FeedbackS__Custo__36B12243");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.FeedbackStaffs)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FeedbackS__Staff__37A5467C");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).HasMaxLength(255);

                entity.Property(e => e.SendDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notificat__Accou__3B75D760");
            });

            modelBuilder.Entity<OwnerAccount>(entity =>
            {
                entity.ToTable("OwnerAccount");

                entity.HasIndex(e => e.AccountId, "UQ__OwnerAcc__349DA5A7DD4F2E94")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.OwnerAccount)
                    .HasForeignKey<OwnerAccount>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OwnerAcco__Accou__2C3393D0");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK__Product__Discoun__4AB81AF0");

                entity.HasOne(d => d.RepairService)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.RepairServiceId)
                    .HasConstraintName("FK__Product__RepairS__49C3F6B7");

                entity.HasOne(d => d.Warranty)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.WarrantyId)
                    .HasConstraintName("FK__Product__Warrant__4BAC3F29");

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.Products)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProductCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductCa__Categ__571DF1D5"),
                        r => r.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductCa__Produ__5629CD9C"),
                        j =>
                        {
                            j.HasKey("ProductId", "CategoryId").HasName("PK__ProductC__159C556D631A1E32");

                            j.ToTable("ProductCategory");
                        });

                entity.HasMany(d => d.Vehicles)
                    .WithMany(p => p.Products)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProductVehicleType",
                        l => l.HasOne<Vehicle>().WithMany().HasForeignKey("VehicleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductVe__Vehic__5AEE82B9"),
                        r => r.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductVe__Produ__59FA5E80"),
                        j =>
                        {
                            j.HasKey("ProductId", "VehicleId").HasName("PK__ProductV__807A738414A27C65");

                            j.ToTable("ProductVehicleType");
                        });
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImage");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__Produ__52593CB8");
            });

            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity.ToTable("ProductOrder");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.CancellationDate).HasColumnType("datetime");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ProductOrders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductOr__Custo__6B24EA82");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.ProductOrders)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__ProductOr__Staff__6C190EBB");
            });

            modelBuilder.Entity<ProductOrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.ProductOrderId, e.ProductId })
                    .HasName("PK__ProductO__E4A5EA899BF0E2C1");

                entity.ToTable("ProductOrderDetail");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductOrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductOr__Produ__70DDC3D8");

                entity.HasOne(d => d.ProductOrder)
                    .WithMany(p => p.ProductOrderDetails)
                    .HasForeignKey(d => d.ProductOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductOr__Produ__6FE99F9F");
            });

            modelBuilder.Entity<ProductPrice>(entity =>
            {
                entity.ToTable("ProductPrice");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateApply).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPrices)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductPr__Produ__4F7CD00D");
            });

            modelBuilder.Entity<RepairOrder>(entity =>
            {
                entity.ToTable("RepairOrder");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CustomerName).HasMaxLength(100);

                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.RepairOrders)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RepairOrd__Staff__74AE54BC");
            });

            modelBuilder.Entity<RepairOrderDetail>(entity =>
            {
                entity.ToTable("RepairOrderDetail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RepairCount).HasDefaultValueSql("((1))");

                entity.Property(e => e.WarrantyPeriod).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.RepairOrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__RepairOrd__Produ__7A672E12");

                entity.HasOne(d => d.RepairOrder)
                    .WithMany(p => p.RepairOrderDetails)
                    .HasForeignKey(d => d.RepairOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RepairOrd__Repai__787EE5A0");

                entity.HasOne(d => d.RepairService)
                    .WithMany(p => p.RepairOrderDetails)
                    .HasForeignKey(d => d.RepairServiceId)
                    .HasConstraintName("FK__RepairOrd__Repai__797309D9");
            });

            modelBuilder.Entity<RepairService>(entity =>
            {
                entity.ToTable("RepairService");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<StaffAccount>(entity =>
            {
                entity.ToTable("StaffAccount");

                entity.HasIndex(e => e.AccountId, "UQ__StaffAcc__349DA5A74A276CA1")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.StaffAccount)
                    .HasForeignKey<StaffAccount>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StaffAcco__Accou__300424B4");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("Vehicle");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.VehicleName).HasMaxLength(100);
            });

            modelBuilder.Entity<Warranty>(entity =>
            {
                entity.ToTable("Warranty");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
