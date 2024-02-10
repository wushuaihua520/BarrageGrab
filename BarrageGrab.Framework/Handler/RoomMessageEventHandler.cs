using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Framework.Handler
{
    public delegate void RoomMessageEventHandler(object? sender, RoomMessageEventArgs<object> e);
}
