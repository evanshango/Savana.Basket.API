using System.Net;
using Microsoft.AspNetCore.Mvc;
using Savana.Basket.API.Dtos;
using Savana.Basket.API.Interfaces;

namespace Savana.Basket.API.Controllers;

[ApiController, Route("recent/{historyId}"), Produces("application/json"), Tags("Recent")]
public class HistoryController : ControllerBase {
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService) => _historyService = historyService;

    [HttpGet, ProducesResponseType(typeof(RecentDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<RecentDto>>> GetRecentItems([FromRoute] string historyId) {
        var items = await _historyService.GetRecentItems(historyId);
        return Ok(items ?? new RecentDto { HistoryId = historyId });
    }

    [HttpPost("")]
    public async Task<ActionResult<RecentDto>> UpsertHistory([FromRoute] string historyId, [FromBody] Product product) {
        var existing = await _historyService.GetRecentItems(historyId);
        var updated = await _historyService.UpsertHistory(product, existing, historyId);
        return Ok(updated);
    }

    [HttpDelete("")]
    public async Task<IActionResult> DeleteHistory([FromRoute] string historyId) {
        await _historyService.DeleteHistory(historyId);
        return NoContent();
    }
}