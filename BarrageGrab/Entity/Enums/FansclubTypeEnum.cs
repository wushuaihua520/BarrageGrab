using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Enums
{
    /// <summary>
    /// 粉丝团消息类型
    /// </summary>
    public enum FansclubTypeEnum
    {
        [Description("粉丝团升级")]
        UpGrade = 1,

        [Description("加入粉丝团")]
        Join,
    }
}
