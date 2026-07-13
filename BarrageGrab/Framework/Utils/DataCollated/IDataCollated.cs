using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Framework.Utils.DataCollated
{
    public interface IDataCollated
    {
        T GetUser<T>(object user);
    }
}
