using BarrageGrab.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Framework
{
    public class RoomMessageEventArgs<T> : EventArgs where T : class
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypeEnum? Type { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public T? Message { get; set; }


        public RoomMessageEventArgs()
        {

        }

        public RoomMessageEventArgs(MessageTypeEnum? type, T message)
        {
            this.Type = type;
            this.Message = message;
        }
    }
}
