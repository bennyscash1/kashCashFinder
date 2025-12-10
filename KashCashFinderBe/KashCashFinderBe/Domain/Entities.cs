using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KashCashFinderBe.Domain;

public class Category
{
    public int CategoryId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Category? ParentCategory { get; set; }

    public ICollection<Category> Children { get; set; } = new List<Category>();

    public ICollection<CategorySynonym> Synonyms { get; set; } = new List<CategorySynonym>();

    public ICollection<StoreCategory> StoreCategories { get; set; } = new List<StoreCategory>();
}

public class CategorySynonym
{
    public int CategorySynonymId { get; set; }

    public int CategoryId { get; set; }

    [MaxLength(100)]
    public string Synonym { get; set; } = string.Empty;

    public Category Category { get; set; } = null!;
}

public class Store
{
    public int StoreId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? AddressLine1 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal? Longitude { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? WebsiteUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<StoreCategory> StoreCategories { get; set; } = new List<StoreCategory>();

    public ICollection<StoreTag> Tags { get; set; } = new List<StoreTag>();
}

public class StoreCategory
{
    public int StoreId { get; set; }

    public int CategoryId { get; set; }

    public Store Store { get; set; } = null!;

    public Category Category { get; set; } = null!;
}

public class StoreTag
{
    public int StoreTagId { get; set; }

    public int StoreId { get; set; }

    [MaxLength(100)]
    public string Tag { get; set; } = string.Empty;

    public Store Store { get; set; } = null!;
}




