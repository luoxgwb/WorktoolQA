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
        [HttpPost]
        public IActionResult Test1(RequestDto requestDto)
        {
            string json = "{\"code\":0,\"message\":\"success\",\"data\":{\"type\":5000,\"info\":{\"text\":\"你也好啊\"}}}";

            var res = JsonConvert.DeserializeObject<ResultDto>(json);

            return Ok(res);
        }

        [HttpPost]
        public IActionResult Test2(RequestDto requestDto)
        {
            string json = "{\"code\":200,\"message\":\"操作成功\",\"data\":{\"type\":5000,\"info\":{\"text\":\"1\"},\"text\":\"2\"}}";

            //var res = JsonConvert.DeserializeObject<ResultDto>(json);

            return Ok(json);
        }
    }
}
