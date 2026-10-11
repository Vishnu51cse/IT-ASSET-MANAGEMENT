using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/asset-history")]
    [Authorize]
    public class AssetHistoryController : ControllerBase
    {
        private readonly IAssetHistoryService _historyService;

        public AssetHistoryController(IAssetHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHistories()
        {
            var histories = await _historyService.GetAllHistoriesAsync();
            return Ok(histories);
        }

        [HttpGet("asset/{assetId}")]
        public async Task<IActionResult> GetHistoryByAssetId(int assetId)
        {
            var histories = await _historyService.GetHistoryByAssetIdAsync(assetId);
            return Ok(histories);
        }
    }
}
