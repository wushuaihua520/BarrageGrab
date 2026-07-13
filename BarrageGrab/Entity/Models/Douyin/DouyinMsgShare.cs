using BarrageGrab.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Models.Douyin
{
    /// <summary>
    /// 抖音分享消息
    /// </summary>
    public class DouyinMsgShare : DouyinMsgBase
    {
        /// <summary>
        /// 分享目标
        /// </summary>
        public ShareTypeEnum ShareType { get; set; }
    }
}
