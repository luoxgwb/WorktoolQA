using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
