using KashCashFinderBe.Contracts;
using KashCashFinderBe.Data;
using Microsoft.EntityFrameworkCore;

namespace KashCashFinderBe.Services;

public interface ISearchService
{
    Task<IReadOnlyList<SearchResultItem>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SearchResultItem>> GetAllCompaniesAsync(int? maxResults = null, CancellationToken cancellationToken = default);
}

public class SearchService : ISearchService
{
    private readonly WhereCanBuyDbContext _db;

    public SearchService(WhereCanBuyDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SearchResultItem>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return Array.Empty<SearchResultItem>();
        }

        var normalizedQuery = request.Query.Trim().ToLowerInvariant();

        var baseQuery = _db.Stores
            .AsNoTracking()
            .Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToLowerInvariant();
            baseQuery = baseQuery.Where(s => s.City != null && s.City.ToLower() == city);
        }

        if (request.CategoryIds is { Count: > 0 })
        {
            var categoryIds = request.CategoryIds;
            baseQuery = baseQuery.Where(s => s.StoreCategories.Any(sc => categoryIds.Contains(sc.CategoryId)));
        }

        var storesWithRelations = await baseQuery
            .Include(s => s.StoreCategories)
                .ThenInclude(sc => sc.Category)
            .Include(s => s.Tags)
            .ToListAsync(cancellationToken);

        var results = new List<SearchResultItem>();

        foreach (var store in storesWithRelations)
        {
            double score = 0;
            var name = store.Name.ToLowerInvariant();
            var description = store.Description?.ToLowerInvariant() ?? string.Empty;

            if (name.Contains(normalizedQuery))
            {
                score += 60;
            }

            if (!string.IsNullOrEmpty(description) && description.Contains(normalizedQuery))
            {
                score += 10;
            }

            var tagsText = string.Join(' ', store.Tags.Select(t => t.Tag.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(tagsText) && tagsText.Contains(normalizedQuery))
            {
                score += 20;
            }

            var categories = store.StoreCategories.Select(sc => sc.Category).Where(c => c != null).ToList()!;
            var categoryText = string.Join(' ', categories.Select(c => c.Name.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(categoryText) && categoryText.Contains(normalizedQuery))
            {
                score += 30;
            }

            // Very simple normalization to 0–100 range and minimum threshold.
            if (score <= 0)
            {
                continue;
            }

            var result = new SearchResultItem
            {
                StoreId = store.StoreId,
                Name = store.Name,
                Categories = categories.Select(c => c!.Name).Distinct().ToList(),
                Address = store.AddressLine1,
                Score = Math.Min(score, 100)
            };

            results.Add(result);
        }

        var ordered = results
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Name)
            .ToList();

        var maxResults = request.MaxResults.GetValueOrDefault(20);
        if (maxResults > 0)
        {
            ordered = ordered.Take(maxResults).ToList();
        }

        return ordered;
    }

    public async Task<IReadOnlyList<SearchResultItem>> GetAllCompaniesAsync(int? maxResults = null, CancellationToken cancellationToken = default)
    {
        var baseQuery = _db.Stores
            .AsNoTracking()
            .Where(s => s.IsActive);

        var storesWithRelations = await baseQuery
            .Include(s => s.StoreCategories)
                .ThenInclude(sc => sc.Category)
            .ToListAsync(cancellationToken);

        var results = new List<SearchResultItem>();

        foreach (var store in storesWithRelations)
        {
            var categories = store.StoreCategories.Select(sc => sc.Category).Where(c => c != null).ToList()!;

            var result = new SearchResultItem
            {
                StoreId = store.StoreId,
                Name = store.Name,
                Categories = categories.Select(c => c!.Name).Distinct().ToList(),
                Address = store.AddressLine1,
                Score = 0
            };

            results.Add(result);
        }

        var ordered = results
            .OrderBy(r => r.Name)
            .ToList();

        var effectiveMaxResults = maxResults.GetValueOrDefault(0);
        if (effectiveMaxResults > 0)
        {
            ordered = ordered.Take(effectiveMaxResults).ToList();
        }

        return ordered;
    }
}




