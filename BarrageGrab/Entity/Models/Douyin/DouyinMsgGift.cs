using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Models.Douyin
{
    /// <summary>
    /// 抖音礼物消息
    /// </summary>
    public class DouyinMsgGift : DouyinMsgBase
    {
        /// <summary>
        /// 礼物ID
        /// </summary>
        public long GiftId { get; set; }

        /// <summary>
        /// 礼物名称
        /// </summary>
        public string? GiftName { get; set; }

        /// <summary>
        /// 礼物分组ID
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 本次(增量)礼物数量
        /// </summary>
        //public long GiftCount { get; set; }

        public long ComboCount { get; set; }

        public long GroupCount { get; set; }

        public long TotalCount { get; set; }

        public int RepeatEnd { get; set; }

        /// <summary>
        /// 礼物数量(连续的)
        /// </summary>
        public long RepeatCount { get; set; }

        /// <summary>
        /// 抖币价格
        /// </summary>
        public int DiamondCount { get; set; }

        /// <summary>
        /// 送礼目标(连麦直播间有用)
        /// </summary>
        public DouyinUser? ToUser { get; set; }
    }
}
