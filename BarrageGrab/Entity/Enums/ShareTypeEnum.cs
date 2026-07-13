using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Enums
{
    /// <summary>
    /// 直播间分享目标
    /// </summary>
    public enum ShareTypeEnum
    {
        [Description("微信")]
        Wechat = 1,

        [Description("朋友圈")]
        CircleOfFriends,

        [Description("微博")]
        Weibo = 3,

        [Description("QQ空间")]
        Qzone = 4,

        [Description("QQ")]
        QQ = 5,

        [Description("抖音好友")]
        Douyin = 112
    }
}
