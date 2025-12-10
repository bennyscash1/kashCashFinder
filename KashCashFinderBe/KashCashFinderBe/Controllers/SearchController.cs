using KashCashFinderBe.Contracts;
using KashCashFinderBe.Services;
using Microsoft.AspNetCore.Mvc;

namespace KashCashFinderBe.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpOptions]
    public IActionResult Options()
    {
        // Handles CORS preflight requests
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<SearchResultItem>>> SearchAsync([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Query is required.");
        }

        var results = await _searchService.SearchAsync(request, cancellationToken);
        return Ok(new { results });
    }

    [HttpGet("all")]
    public async Task<ActionResult<IReadOnlyList<SearchResultItem>>> GetAllAsync([FromQuery] int? maxResults, CancellationToken cancellationToken)
    {
        var results = await _searchService.GetAllCompaniesAsync(maxResults, cancellationToken);
        return Ok(new { results });
    }
}




