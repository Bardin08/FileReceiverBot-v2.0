using System.Threading.Tasks;

using FileReceiverBot.Api.Services.Abstract;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

namespace FileReceiverBot.Api.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("api/bot1805413036")]
    public class BotController : Controller
    {
        private readonly IUpdateHandlerService _updateHandlerServiceService;

        public BotController(IUpdateHandlerService updateHandlerService)
        {
            _updateHandlerServiceService = updateHandlerService;
        }

        [HttpPost("/update")]
        public async Task<IActionResult> ProcessTelegramMessage([FromBody] Update update)
        {
            await _updateHandlerServiceService.HandleUpdateAsync(update);
            return Ok();
        }
    }
}