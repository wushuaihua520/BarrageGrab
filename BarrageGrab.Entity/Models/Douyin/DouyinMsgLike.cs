using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Models.Douyin
{
    /// <summary>
    /// 抖音点赞消息
    /// </summary>
    public class DouyinMsgLike : DouyinMsgBase
    {
        /// <summary>
        /// 点赞数量
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// 总共点赞数量
        /// </summary>
        public long Total { get; set; }
    }
}
