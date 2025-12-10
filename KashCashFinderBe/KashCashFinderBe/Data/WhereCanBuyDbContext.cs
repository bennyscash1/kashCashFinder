using KashCashFinderBe.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace KashCashFinderBe.Data;

public class WhereCanBuyDbContext : DbContext
{
    public WhereCanBuyDbContext(DbContextOptions<WhereCanBuyDbContext> options) : base(options)
    {
        Console.WriteLine(  "Sdfafd");
        using var conn = new SqlConnection("Server=BENNYS\\SQLEXPRESS;Database=KashCashFinderDev;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
        conn.Open();
        Console.WriteLine("Connected!");
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategorySynonym> CategorySynonyms => Set<CategorySynonym>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreCategory> StoreCategories => Set<StoreCategory>();
    public DbSet<StoreTag> StoreTags => Set<StoreTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.CategoryId);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Description)
                  .HasMaxLength(255);

            entity.HasOne(e => e.ParentCategory)
                  .WithMany(e => e.Children)
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CategorySynonym>(entity =>
        {
            entity.ToTable("CategorySynonyms");
            entity.HasKey(e => e.CategorySynonymId);

            entity.Property(e => e.Synonym)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Synonyms)
                  .HasForeignKey(e => e.CategoryId);
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Stores");
            entity.HasKey(e => e.StoreId);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Description)
                  .HasMaxLength(2000);

            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(200);
        });

        modelBuilder.Entity<StoreCategory>(entity =>
        {
            entity.ToTable("StoreCategories");
            entity.HasKey(e => new { e.StoreId, e.CategoryId });

            entity.HasOne(e => e.Store)
                  .WithMany(e => e.StoreCategories)
                  .HasForeignKey(e => e.StoreId);

            entity.HasOne(e => e.Category)
                  .WithMany(e => e.StoreCategories)
                  .HasForeignKey(e => e.CategoryId);
        });

        modelBuilder.Entity<StoreTag>(entity =>
        {
            entity.ToTable("StoreTags");
            entity.HasKey(e => e.StoreTagId);

            entity.Property(e => e.Tag)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasOne(e => e.Store)
                  .WithMany(e => e.Tags)
                  .HasForeignKey(e => e.StoreId);
        });
    }
}




