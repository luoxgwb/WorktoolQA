using Dto;

namespace Services.IReplyServices
{
    public interface IMessageService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="requestViewModel"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<ResultDto> SendMessageAsync(RequestDto requestDto);
    }
}
