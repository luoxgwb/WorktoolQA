using Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.IReplyServices;

namespace WorkToolChatGPTAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<ReplyController> _logger;

        public ReplyController(IMessageService configuration, ILogger<ReplyController> logger)
        {
            _messageService = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Message([FromBody] RequestDto requestDto)
        {
            var res = await _messageService.SendMessageAsync(requestDto);
            return Ok(res);
        }


    }
}
