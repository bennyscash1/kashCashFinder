using KashCashFinderBe.Contracts;
using KashCashFinderBe.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KashCashFinderBe.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly WhereCanBuyDbContext _db;

    public CategoriesController(WhereCanBuyDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId
            })
            .ToListAsync(cancellationToken);

        return Ok(categories);
    }
}




