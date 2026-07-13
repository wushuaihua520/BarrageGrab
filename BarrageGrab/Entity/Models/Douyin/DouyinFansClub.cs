using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Models.Douyin
{
    public class DouyinFansClub
    {
        /// <summary>
        /// 粉丝团名称
        /// </summary>
        public string? ClubName { get; set; }

        /// <summary>
        /// 粉丝团等级，没加入则0
        /// </summary>
        public int Level { get; set; }
    }
}
