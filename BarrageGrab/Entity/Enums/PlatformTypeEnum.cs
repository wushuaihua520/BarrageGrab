using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Enums
{
    /// <summary>
    /// 平台
    /// </summary>
    public enum PlatformTypeEnum
    {
        [Description("抖音")]
        Douyin = 1,

        [Description("Tiktok")]
        Tiktok,

        [Description("快手")]
        Kuaishou,

        [Description("哔哩哔哩")]
        Bilibili,

        [Description("小红书")]
        Xiaohongshu,

        [Description("视频号")]
        Shipinhao,
    }
}
