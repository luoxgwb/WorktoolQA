using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Diagnostics;
using System.Text.Json;
using WorkToolChatGPTAPI.Model;

namespace WorkToolChatGPTAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        public ReplyController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        [HttpPost]
        public async Task<IActionResult> Message(RequestViewModel requestViewModel)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var res = new ResultViewModel();
            var MaxCount = Configuration.GetSection("ChatGPT")["MaxCount"];

            if (GlobalModel.Count > Convert.ToInt32(MaxCount))
            {
                res.Data.Info.Text = "次数已达上限";
                return new JsonResult(res);
            }

            var key = Configuration.GetSection("ChatGPT")["Key"];

            var conversationDic = GlobalModel.ConversationDic;

            var conversationKey = requestViewModel.ReceivedName + requestViewModel.GroupName + requestViewModel.GroupRemark;

            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(key));
            {

                Conversation chat;
                if (conversationDic.ContainsKey(conversationKey))
                {
                    chat = conversationDic[conversationKey];
                }
                else
                {
                    chat = api.Chat.CreateConversation();
                    conversationDic.Add(conversationKey, chat);
                }

                chat.AppendUserInput(requestViewModel.Spoken);
                try
                {
                    res.Data.Info.Text = await chat.GetResponseFromChatbot();
                    GlobalModel.Count++;
                }
                catch (Exception ex)
                {
                    res.Code = 500;
                    res.Message = ex.Message;
                }
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            if (ts.TotalSeconds > 2)
            {
                var sendViewModel = new SendViewModel();
                var newList = new List
                {
                    ReceivedContent = res.Data.Info.Text,
                    TitleList = new[] { requestViewModel.GroupName },
                    AtList = new[] { requestViewModel.ReceivedName }
                };
                sendViewModel.List = sendViewModel.List.Append(newList).ToArray();

                var url = "https://worktool.asrtts.cn/wework/sendRawMessage?robotId=" + Configuration.GetSection("ChatGPT")["RobotId"];
                var client = new HttpClient();
                HttpContent content = new StringContent(JsonSerializer.Serialize(sendViewModel));
                await client.PostAsync(url, content);

            }
            return new JsonResult(res);
        }
    }
}
