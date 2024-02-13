using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Models.Douyin
{
    /// <summary>
    /// 抖音用户
    /// </summary>
    public class DouyinUser : DouyinMsgBase
    {
        /// <summary>
        /// 真实ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ShortId
        /// </summary>
        public long ShortId { get; set; }

        /// <summary>
        /// 自定义ID
        /// </summary>
        public string? DisplayId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 未知
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 支付等级
        /// </summary>
        public int PayLevel { get; set; }

        /// <summary>
        /// 性别 1男 2女
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public long Birthday { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string? Telephone { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 用户主页地址
        /// </summary>
        public string? SecUid { get; set; }

        /// <summary>
        /// 粉丝团信息
        /// </summary>
        public DouyinFansClub? FansClub { get; set; }

        /// <summary>
        /// 粉丝数
        /// </summary>
        public long FollowerCount { get; set; }

        /// <summary>
        /// 关注状态 0 未关注 1 已关注 2,不明
        /// </summary>
        public long FollowStatus { get; set; }

        /// <summary>
        /// 关注数
        /// </summary>
        public long FollowingCount;

    }
}
