using BarrageGrab.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Models
{
    /// <summary>
    /// 弹幕消息实体模型
    /// </summary>
    public class BarrageMessageModel
    {
        public MessageTypeEnum MessageType { get; set; }

        public object? Message { get; set; }
    }
}
