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
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Utility.Enums;

namespace Services.ReplyServices
{
    public class MessageService : IMessageService, IScoped
    {
        public MessageService(IConfiguration configuration, IMemoryCache memoryCache, SqlDbContext context, ILogger<MessageService> logger)
        {
            Configuration = configuration;
            _memoryCache = memoryCache;
            _context = context;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }
        private readonly IMemoryCache _memoryCache;
        private readonly SqlDbContext _context;
        private readonly ILogger<MessageService> _logger;

        public async Task<ResultDto> SendMessageAsync(RequestDto requestDto)
        {
            var OperationID = Guid.NewGuid();

            var option = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var workToolLogJson = new WorkToolLog
            {
                OperationID = OperationID,
                Type = LogType.System,
                ReceivedName = requestDto.ReceivedName,
                GroupName = requestDto.GroupName,
                GroupRemark = requestDto.GroupRemark,
                Message = JsonSerializer.Serialize(requestDto, option)
            };

            _context.WorkToolLog.Add(workToolLogJson);

            var workToolLogUser = new WorkToolLog
            {
                OperationID = OperationID,
                Type = LogType.User,
                ReceivedName = requestDto.ReceivedName,
                GroupName = requestDto.GroupName,
                GroupRemark = requestDto.GroupRemark,
                Message = requestDto.Spoken
            };

            _context.WorkToolLog.Add(workToolLogUser);


            _logger.LogInformation("进入");


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

                var workToolLogUpperLimit = new WorkToolLog
                {
                    OperationID = OperationID,
                    Type = LogType.System,
                    ReceivedName = requestDto.ReceivedName,
                    GroupName = requestDto.GroupName,
                    GroupRemark = requestDto.GroupRemark,
                    Message = res.Data.Info.Text
                };

                _context.WorkToolLog.Add(workToolLogUpperLimit);

                await _context.SaveChangesAsync();
                return res;
            }

            var key = Configuration.GetSection("ChatGPT")["Key"];

            _logger.LogInformation("Openai Key: {key}", key);

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
                    var workToolLogSend = new WorkToolLog
                    {
                        OperationID = OperationID,
                        Type = LogType.System,
                        ReceivedName = requestDto.ReceivedName,
                        GroupName = requestDto.GroupName,
                        GroupRemark = requestDto.GroupRemark,
                        Message = "为gpt发送消息"
                    };

                    _context.WorkToolLog.Add(workToolLogSend);

                    res.Data.Info.Text = await chat.GetResponseFromChatbotAsync();
                    count++;

                    var workToolLogReceive = new WorkToolLog
                    {
                        OperationID = OperationID,
                        Type = LogType.Bot,
                        ReceivedName = requestDto.ReceivedName,
                        GroupName = requestDto.GroupName,
                        GroupRemark = requestDto.GroupRemark,
                        Message = res.Data.Info.Text
                    };

                    _context.WorkToolLog.Add(workToolLogReceive);

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

                    //await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    res.Code = 500;
                    res.Message = ex.Message;
                    _logger.LogInformation(ex.Message);

                    var workToolLogError = new WorkToolLog
                    {
                        OperationID = OperationID,
                        Type = LogType.System,
                        ReceivedName = requestDto.ReceivedName,
                        GroupName = requestDto.GroupName,
                        GroupRemark = requestDto.GroupRemark,
                        Message = ex.Message
                    };

                    _context.WorkToolLog.Add(workToolLogError);
                    await _context.SaveChangesAsync();
                    return res;
                }
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            if (ts.TotalSeconds > 2)
            {
                _logger.LogInformation("启用发送指令");
                var workToolLogError = new WorkToolLog
                {
                    OperationID = OperationID,
                    Type = LogType.System,
                    ReceivedName = requestDto.ReceivedName,
                    GroupName = requestDto.GroupName,
                    GroupRemark = requestDto.GroupRemark,
                    Message = "启用发送指令"
                };

                _context.WorkToolLog.Add(workToolLogError);

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
                var sendRawResult = await client.PostAsync(url, content);

                _logger.LogInformation("发送指令结束");

                var workToolLogSendRawResult = new WorkToolLog
                {
                    OperationID = OperationID,
                    Type = LogType.System,
                    ReceivedName = requestDto.ReceivedName,
                    GroupName = requestDto.GroupName,
                    GroupRemark = requestDto.GroupRemark,
                    Message = "发送指令结束" + sendRawResult.StatusCode
                };
                _context.WorkToolLog.Add(workToolLogSendRawResult);

                await _context.SaveChangesAsync();
            }
            return res;
        }
    }
}
