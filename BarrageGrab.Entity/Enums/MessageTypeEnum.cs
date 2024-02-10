using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Enums
{
    /// <summary>
    /// 消息类型
    /// （目前暂为抖音消息类型）
    /// </summary>
    public enum MessageTypeEnum
    {
        /// <summary>
        /// 进入
        /// </summary>
        [Description("进入")]
        Member = 1,

        /// <summary>
        /// 关注
        /// </summary>
        [Description("关注")]
        Social,

        /// <summary>
        /// 弹幕
        /// </summary>
        [Description("弹幕")]
        Chat,

        /// <summary>
        /// 点赞
        /// </summary>
        [Description("点赞")]
        Like,

        /// <summary>
        /// 礼物
        /// </summary>
        [Description("礼物")]
        Gift,

        /// <summary>
        /// 分享
        /// </summary>
        [Description("分享")]
        Share,

        /// <summary>
        /// 统计
        /// </summary>
        [Description("统计")]
        RoomUserSeq,

        /// <summary>
        /// 状态变更
        /// </summary>
        [Description("状态变更")]
        Control,

        /// <summary>
        /// 粉丝团
        /// </summary>
        [Description("粉丝团")]
        Fansclub,

        /// <summary>
        /// 直播间状态
        /// </summary>
        [Description("直播间状态")]
        RoomStats,
    }
}
