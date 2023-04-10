using Dto;
using Microsoft.AspNetCore.Mvc;
using Services.IReplyServices;

namespace WorkToolChatGPTAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ReplyController(IMessageService configuration)
        {
            _messageService = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Message(RequestDto requestDto)
        {
            var res = await _messageService.SendMessageAsync(requestDto);
            return Ok(res);
        }
    }
}
