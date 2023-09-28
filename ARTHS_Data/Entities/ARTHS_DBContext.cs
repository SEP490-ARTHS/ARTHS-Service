﻿using System;
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
        public virtual DbSet<InStoreOrder> InStoreOrders { get; set; } = null!;
        public virtual DbSet<InStoreOrderDetail> InStoreOrderDetails { get; set; } = null!;
        public virtual DbSet<MotobikeProduct> MotobikeProducts { get; set; } = null!;
        public virtual DbSet<MotobikeProductImage> MotobikeProductImages { get; set; } = null!;
        public virtual DbSet<MotobikeProductPrice> MotobikeProductPrices { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<OnlineOrder> OnlineOrders { get; set; } = null!;
        public virtual DbSet<OnlineOrderDetail> OnlineOrderDetails { get; set; } = null!;
        public virtual DbSet<OwnerAccount> OwnerAccounts { get; set; } = null!;
        public virtual DbSet<RepairBooking> RepairBookings { get; set; } = null!;
        public virtual DbSet<RepairService> RepairServices { get; set; } = null!;
        public virtual DbSet<StaffAccount> StaffAccounts { get; set; } = null!;
        public virtual DbSet<TellerAccount> TellerAccounts { get; set; } = null!;
        public virtual DbSet<Vehicle> Vehicles { get; set; } = null!;
        public virtual DbSet<Warranty> Warranties { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=TAN-TRUNG\\HAMMER;Database=ARTHS_DB;Persist Security Info=False;User ID=sa;Password=123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.PhoneNumber, "UQ__Account__85FB4E38484875CC")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

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

                entity.HasIndex(e => e.InStoreOrderId, "UQ__Bill__AFEA978F0FA40AD6")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BillDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InStoreOrderId)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.HasOne(d => d.InStoreOrder)
                    .WithOne(p => p.Bill)
                    .HasForeignKey<Bill>(d => d.InStoreOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bill__InStoreOrd__7E37BEF6");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.HasIndex(e => e.CustomerId, "UQ__Cart__A4AE64D9F9C93100")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.Cart)
                    .HasForeignKey<Cart>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cart__CustomerId__60A75C0F");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => new { e.CartId, e.MotobikeProductId })
                    .HasName("PK__CartItem__4B299AA3F43705C2");

                entity.ToTable("CartItem");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CartItem__CartId__6383C8BA");

                entity.HasOne(d => d.MotobikeProduct)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.MotobikeProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CartItem__Motobi__6477ECF3");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CategoryName).HasMaxLength(100);
            });

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PK__Customer__349DA5A6E6A28A49");

                entity.ToTable("CustomerAccount");

                entity.Property(e => e.AccountId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

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
                    .HasConstraintName("FK__FeedbackP__Custo__5AEE82B9");

                entity.HasOne(d => d.MotobikeProduct)
                    .WithMany(p => p.FeedbackProducts)
                    .HasForeignKey(d => d.MotobikeProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FeedbackP__Motob__5BE2A6F2");
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

            modelBuilder.Entity<InStoreOrder>(entity =>
            {
                entity.ToTable("InStoreOrder");

                entity.Property(e => e.Id)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerName).HasMaxLength(100);

                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrderType).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.InStoreOrders)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InStoreOr__Staff__72C60C4A");

                entity.HasOne(d => d.Teller)
                    .WithMany(p => p.InStoreOrders)
                    .HasForeignKey(d => d.TellerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InStoreOr__Telle__71D1E811");
            });

            modelBuilder.Entity<InStoreOrderDetail>(entity =>
            {
                entity.ToTable("InStoreOrderDetail");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InStoreOrderId)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RepairCount).HasDefaultValueSql("((1))");

                entity.Property(e => e.WarrantyPeriod).HasColumnType("datetime");

                entity.HasOne(d => d.InStoreOrder)
                    .WithMany(p => p.InStoreOrderDetails)
                    .HasForeignKey(d => d.InStoreOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InStoreOr__InSto__76969D2E");

                entity.HasOne(d => d.MotobikeProduct)
                    .WithMany(p => p.InStoreOrderDetails)
                    .HasForeignKey(d => d.MotobikeProductId)
                    .HasConstraintName("FK__InStoreOr__Motob__787EE5A0");

                entity.HasOne(d => d.RepairService)
                    .WithMany(p => p.InStoreOrderDetails)
                    .HasForeignKey(d => d.RepairServiceId)
                    .HasConstraintName("FK__InStoreOr__Repai__778AC167");
            });

            modelBuilder.Entity<MotobikeProduct>(entity =>
            {
                entity.ToTable("MotobikeProduct");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.MotobikeProducts)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__MotobikeP__Categ__4CA06362");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.MotobikeProducts)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK__MotobikeP__Disco__4AB81AF0");

                entity.HasOne(d => d.RepairService)
                    .WithMany(p => p.MotobikeProducts)
                    .HasForeignKey(d => d.RepairServiceId)
                    .HasConstraintName("FK__MotobikeP__Repai__49C3F6B7");

                entity.HasOne(d => d.Warranty)
                    .WithMany(p => p.MotobikeProducts)
                    .HasForeignKey(d => d.WarrantyId)
                    .HasConstraintName("FK__MotobikeP__Warra__4BAC3F29");

                entity.HasMany(d => d.Vehicles)
                    .WithMany(p => p.MotobikeProducts)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProductVehicleType",
                        l => l.HasOne<Vehicle>().WithMany().HasForeignKey("VehicleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductVe__Vehic__5812160E"),
                        r => r.HasOne<MotobikeProduct>().WithMany().HasForeignKey("MotobikeProductId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductVe__Motob__571DF1D5"),
                        j =>
                        {
                            j.HasKey("MotobikeProductId", "VehicleId").HasName("PK__ProductV__9D226409F8DF9B54");

                            j.ToTable("ProductVehicleType");
                        });
            });

            modelBuilder.Entity<MotobikeProductImage>(entity =>
            {
                entity.ToTable("MotobikeProductImage");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.HasOne(d => d.MotobikeProduct)
                    .WithMany(p => p.MotobikeProductImages)
                    .HasForeignKey(d => d.MotobikeProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MotobikeP__Motob__534D60F1");
            });

            modelBuilder.Entity<MotobikeProductPrice>(entity =>
            {
                entity.ToTable("MotobikeProductPrice");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateApply).HasColumnType("datetime");

                entity.HasOne(d => d.MotobikeProduct)
                    .WithMany(p => p.MotobikeProductPrices)
                    .HasForeignKey(d => d.MotobikeProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MotobikeP__Motob__5070F446");
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

            modelBuilder.Entity<OnlineOrder>(entity =>
            {
                entity.ToTable("OnlineOrder");

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
                    .WithMany(p => p.OnlineOrders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OnlineOrd__Custo__68487DD7");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.OnlineOrders)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__OnlineOrd__Staff__693CA210");
            });

            modelBuilder.Entity<OnlineOrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OnlineOrderId, e.MotobikeProductId })
                    .HasName("PK__OnlineOr__086FF0395ED03FA5");

                entity.ToTable("OnlineOrderDetail");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MotobikeProduct)
                    .WithMany(p => p.OnlineOrderDetails)
                    .HasForeignKey(d => d.MotobikeProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OnlineOrd__Motob__6E01572D");

                entity.HasOne(d => d.OnlineOrder)
                    .WithMany(p => p.OnlineOrderDetails)
                    .HasForeignKey(d => d.OnlineOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OnlineOrd__Onlin__6D0D32F4");
            });

            modelBuilder.Entity<OwnerAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PK__OwnerAcc__349DA5A63AFE85AF");

                entity.ToTable("OwnerAccount");

                entity.Property(e => e.AccountId).ValueGeneratedNever();

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.OwnerAccount)
                    .HasForeignKey<OwnerAccount>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OwnerAcco__Accou__2B3F6F97");
            });

            modelBuilder.Entity<RepairBooking>(entity =>
            {
                entity.ToTable("RepairBooking");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CancellationDate).HasColumnType("datetime");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateBook).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RepairBookings)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RepairBoo__Custo__02084FDA");
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
            });

            modelBuilder.Entity<StaffAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PK__StaffAcc__349DA5A671B0AB8D");

                entity.ToTable("StaffAccount");

                entity.Property(e => e.AccountId).ValueGeneratedNever();

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.StaffAccount)
                    .HasForeignKey<StaffAccount>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StaffAcco__Accou__2E1BDC42");
            });

            modelBuilder.Entity<TellerAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PK__TellerAc__349DA5A676190985");

                entity.ToTable("TellerAccount");

                entity.Property(e => e.AccountId).ValueGeneratedNever();

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.TellerAccount)
                    .HasForeignKey<TellerAccount>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TellerAcc__Accou__30F848ED");
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
