using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Models.Douyin
{
    public class DouyinMsgBase
    {
        /// <summary>
        /// 弹幕ID
        /// </summary>
        public long MsgId { get; set; }

        /// <summary>
        /// 用户数据
        /// </summary>
        public DouyinUser? User { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        public long RoomId { get; set; }

        /// <summary>
        /// web直播间ID
        /// </summary>
        public long WebRoomId { get; set; }
    }
}
