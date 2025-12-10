namespace KashCashFinderBe.Contracts;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public List<int>? CategoryIds { get; set; }
    public string? City { get; set; }
    public int? MaxResults { get; set; }
}

public class SearchResultItem
{
    public int StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public string? Address { get; set; }
    public double Score { get; set; }
}

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
}




