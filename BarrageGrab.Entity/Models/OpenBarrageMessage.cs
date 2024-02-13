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
    public class OpenBarrageMessage
    {
        public MessageTypeEnum Type { get; set; }

        public object? Data { get; set; }
    }
}
