using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;

namespace FileReceiverBot.Api.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("/api/bot1805413036")]
    public class BotController : Controller
    {
        private readonly IUpdateHandlerService _updateHandlerServiceService;
        private readonly ILogger<BotController> _logger;

        public BotController(IUpdateHandlerService updateHandlerService,
            ILogger<BotController> logger)
        {
            _updateHandlerServiceService = updateHandlerService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/update")]
        public async Task<IActionResult> ProcessTelegramMessage([FromBody] Update update)
        {
            _logger.LogInformation("Received a request from the bot!");
            _logger.LogDebug("Received a request from the bot!");
            _logger.LogWarning("Received a request from the bot!");
            _logger.LogError("Received a request from the bot!");
            _logger.LogCritical("Received a request from the bot!");
            _logger.LogTrace("Received a request from the bot!");
            await _updateHandlerServiceService.HandleUpdateAsync(update);
            return Ok();
        }
    }
}
