using BarrageGrab.Entity.Models.Douyin;
using BarrageGrab.Framework.Utils.DataCollated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Framework.Utils
{
    /// <summary>
    /// 数据整理类
    /// </summary>
    public static class DataCollatedUtil
    {

        #region Douyin
        private static IDataCollated? _douyin = null;
        public static IDataCollated? Douyin
        {
            get
            {
                if (_douyin == null)
                {
                    _douyin = new DouyinDataCollated();
                }

                return _douyin;
            }
        }
        #endregion




    }
}
