using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using happinesCafe.Models;

namespace happinesCafe.DATA
{
    public partial class CaffeeSystemContext : DbContext
    {
        public CaffeeSystemContext()
        {
        }

        public CaffeeSystemContext(DbContextOptions<CaffeeSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Basket> Baskets { get; set; } = null!;
        public virtual DbSet<BasketProductAddOn> BasketProductAddOns { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Favorite> Favorites { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<OrderItemsProductAddOn> OrderItemsProductAddOns { get; set; } = null!;
        public virtual DbSet<OrderState> OrderStates { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductAddOn> ProductAddOns { get; set; } = null!;
        public virtual DbSet<ProductAddOnsTyp> ProductAddOnsTyps { get; set; } = null!;
        public virtual DbSet<ProductsSize> ProductsSizes { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserStatus> UserStatuses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=REEM\\G2;database= CaffeeSystem;trusted_connection=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Basket>(entity =>
            {
                entity.HasKey(e => e.IdBasket);

                entity.ToTable("basket");

                entity.Property(e => e.IdBasket).HasColumnName("id_basket");

                entity.Property(e => e.AddedDate)
                    .HasColumnType("date")
                    .HasColumnName("added_date");

                entity.Property(e => e.IdProductSize).HasColumnName("id_productSize");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.QuantityProduct).HasColumnName("quantity_product");

                entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");

                entity.HasOne(d => d.IdProductSizeNavigation)
                    .WithMany(p => p.Baskets)
                    .HasForeignKey(d => d.IdProductSize)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_basket_ProductsSize");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Baskets)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_basket_users");
            });

            modelBuilder.Entity<BasketProductAddOn>(entity =>
            {
                entity.ToTable("basketProductAdd_ons");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdBasket).HasColumnName("Id_basket");

                entity.Property(e => e.IdProductAddOns).HasColumnName("Id_productAdd_ons");

                entity.HasOne(d => d.IdBasketNavigation)
                    .WithMany(p => p.BasketProductAddOns)
                    .HasForeignKey(d => d.IdBasket)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("basketProductAdd_ons_basket");

                entity.HasOne(d => d.IdProductAddOnsNavigation)
                    .WithMany(p => p.BasketProductAddOns)
                    .HasForeignKey(d => d.IdProductAddOns)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("basketProductAdd_ons_ProductAdd_ons");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.IdCategory);

                entity.ToTable("category");

                entity.Property(e => e.IdCategory).HasColumnName("id_category");

                entity.Property(e => e.NameCategory)
                    .HasMaxLength(50)
                    .HasColumnName("name_category");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => e.IdFavorite);

                entity.ToTable("favorite");

                entity.Property(e => e.IdFavorite).HasColumnName("id_favorite");

                entity.Property(e => e.AddDate)
                    .HasColumnType("date")
                    .HasColumnName("add_date");

                entity.Property(e => e.IdProduct).HasColumnName("id_product");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.IdProduct)
                    .HasConstraintName("FK_favorite_product");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_favorite_favorite");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.IdOrder);

                entity.ToTable("orders");

                entity.Property(e => e.IdOrder).HasColumnName("id_order");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city");

                entity.Property(e => e.ContactPhone).HasMaxLength(100);

                entity.Property(e => e.IdState).HasColumnName("id_state");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("order_date");

                entity.Property(e => e.ShippingAddress).HasMaxLength(100);

                entity.Property(e => e.TotalePrice).HasColumnName("totale_price");

                entity.HasOne(d => d.IdStateNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IdState)
                    .HasConstraintName("FK_orders_order_state");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_orders_orders");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.IdOrderItem);

                entity.ToTable("order_items");

                entity.Property(e => e.IdOrderItem).HasColumnName("id_order_item");

                entity.Property(e => e.IdOrder).HasColumnName("id_order");

                entity.Property(e => e.IdProductSize).HasColumnName("id_productSize");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.IdOrderNavigation)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.IdOrder)
                    .HasConstraintName("FK_order_items_orders");

                entity.HasOne(d => d.IdProductSizeNavigation)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.IdProductSize)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_order_items_ProductsSize");
            });

            modelBuilder.Entity<OrderItemsProductAddOn>(entity =>
            {
                entity.ToTable("OrderItems_ProductAdd_ons");

                entity.Property(e => e.IdOrderItem).HasColumnName("Id_Order_Item");

                entity.Property(e => e.IdProductAddOns).HasColumnName("Id_productAdd_ons");

                entity.HasOne(d => d.IdOrderItemNavigation)
                    .WithMany(p => p.OrderItemsProductAddOns)
                    .HasForeignKey(d => d.IdOrderItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItems_ProductAdd_ons_order_items");

                entity.HasOne(d => d.IdProductAddOnsNavigation)
                    .WithMany(p => p.OrderItemsProductAddOns)
                    .HasForeignKey(d => d.IdProductAddOns)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItems_ProductAdd_ons_ProductAdd_ons");
            });

            modelBuilder.Entity<OrderState>(entity =>
            {
                entity.HasKey(e => e.IdState);

                entity.ToTable("order_state");

                entity.Property(e => e.IdState).HasColumnName("id_state");

                entity.Property(e => e.NameState)
                    .HasMaxLength(50)
                    .HasColumnName("name_state");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.IdProduct);

                entity.ToTable("product");

                entity.Property(e => e.IdProduct).HasColumnName("id_product");

                entity.Property(e => e.About)
                    .HasMaxLength(300)
                    .HasColumnName("about");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("created_date");

                entity.Property(e => e.IdCategory).HasColumnName("id_category");

                entity.Property(e => e.NameProduct)
                    .HasMaxLength(50)
                    .HasColumnName("name_product");

                entity.Property(e => e.Picture)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("picture");

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_product_category");
            });

            modelBuilder.Entity<ProductAddOn>(entity =>
            {
                entity.ToTable("ProductAdd_ons");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdType).HasColumnName("Id_type");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.HasOne(d => d.IdTypeNavigation)
                    .WithMany(p => p.ProductAddOns)
                    .HasForeignKey(d => d.IdType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ProductAdd_ons_ProductAdd_onsTyps");
            });

            modelBuilder.Entity<ProductAddOnsTyp>(entity =>
            {
                entity.ToTable("ProductAdd_onsTyps");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ProductsSize>(entity =>
            {
                entity.ToTable("ProductsSize");

                entity.Property(e => e.IdProduct).HasColumnName("Id_product");

                entity.Property(e => e.IdSize).HasColumnName("Id_size");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.ProductsSizes)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ProductsSize_product");

                entity.HasOne(d => d.IdSizeNavigation)
                    .WithMany(p => p.ProductsSizes)
                    .HasForeignKey(d => d.IdSize)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ProductsSize_Size");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.IdReview);

                entity.ToTable("review");

                entity.Property(e => e.IdReview).HasColumnName("id_review");

                entity.Property(e => e.IdProduct).HasColumnName("id_product");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.ReviewDate)
                    .HasColumnType("date")
                    .HasColumnName("reviewDate");

                entity.Property(e => e.Reviewtext)
                    .HasMaxLength(100)
                    .HasColumnName("reviewtext");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_review_product");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_review_users");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.IdRole);

                entity.ToTable("role");

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.Property(e => e.NameRole)
                    .HasMaxLength(50)
                    .HasColumnName("name_role");
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToTable("Size");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.ToTable("users");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.CreatdDate)
                    .HasColumnType("date")
                    .HasColumnName("creatd_date");

                entity.Property(e => e.EmailUser)
                    .HasMaxLength(50)
                    .HasColumnName("email_user");

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.Property(e => e.NameUser)
                    .HasMaxLength(50)
                    .HasColumnName("name_user");

                entity.Property(e => e.PasswordResetToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordUser)
                    .HasMaxLength(50)
                    .HasColumnName("password_user");

                entity.Property(e => e.PictuerUser)
                    .HasMaxLength(200)
                    .HasColumnName("pictuer__user");

                entity.Property(e => e.ResetTokenExpiry).HasColumnType("date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_role");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_User_status");
            });

            modelBuilder.Entity<UserStatus>(entity =>
            {
                entity.ToTable("User_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
