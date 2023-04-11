using Dto;
using Microsoft.Extensions.Configuration;
using OpenAI_API;
using OpenAI_API.Chat;
using Services.IReplyServices;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Utility.Dependencies;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.ReplyServices
{
    public class MessageService : IMessageService, IScoped
    {
        public MessageService(IConfiguration configuration, IMemoryCache memoryCache, SqlDbContext context)
        {
            Configuration = configuration;
            _memoryCache = memoryCache;
            _context = context;
        }
        public IConfiguration Configuration { get; }
        private readonly IMemoryCache _memoryCache;
        private readonly SqlDbContext _context;
        public async Task<ResultDto> SendMessageAsync(RequestDto requestDto)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var res = new ResultDto();
            var maxCount = Configuration.GetSection("ChatGPT")["MaxCount"];
            var countId = Convert.ToInt32(Configuration.GetSection("ChatGPT")["CountId"]);

            var count = 0;

            var model = await _context.WorkToolCount.Where(x => x.CountId == countId).FirstOrDefaultAsync();

            if (model != null)
            {
                count = model.Count;
            }

            if (count > Convert.ToInt32(maxCount))
            {
                res.Data.Info.Text = "次数已达上限";
                return res;
            }

            var key = Configuration.GetSection("ChatGPT")["Key"];


            var conversationKey = requestDto.ReceivedName + requestDto.GroupName + requestDto.GroupRemark;
            var cacheKey = $"conversation_{conversationKey}";


            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(key));
            {

                Conversation chat;

                if (_memoryCache.TryGetValue(cacheKey, out Conversation conversation))
                {
                    chat = conversation;
                }
                else
                {
                    chat = api.Chat.CreateConversation();
                    _memoryCache.Set(cacheKey, chat, TimeSpan.FromMinutes(10));
                }

                chat.AppendUserInput(requestDto.Spoken);
                try
                {
                    res.Data.Info.Text = await chat.GetResponseFromChatbotAsync();
                    count++;

                    if (model != null)
                    {
                        model.Count = count;
                        _context.Attach(model);
                        _context.Entry(model).Property(x => x.Count).IsModified = true;
                    }
                    else
                    {
                        var workToolCount = new WorkToolCount
                        {
                            CountId = countId,
                            Count = count
                        };
                        await _context.AddAsync(workToolCount);
                    }

                    await _context.SaveChangesAsync();
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
                var sendDto = new SendDto();
                var newList = new List
                {
                    ReceivedContent = res.Data.Info.Text,
                    TitleList = new[] { requestDto.GroupName },
                    AtList = new[] { requestDto.ReceivedName }
                };
                sendDto.List = sendDto.List.Append(newList).ToArray();

                var url = "https://worktool.asrtts.cn/wework/sendRawMessage?robotId=" + Configuration.GetSection("ChatGPT")["RobotId"];
                var client = new HttpClient();
                HttpContent content = new StringContent(JsonSerializer.Serialize(sendDto));
                await client.PostAsync(url, content);

            }
            return res;
        }
    }
}
