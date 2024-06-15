using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.GrabServices
{
    /// <summary>
    /// barrage grab service interface
    /// </summary>
    internal interface IBarrageGrabService
    {
        void Start(string liveId);

        void Stop();

        void ReStart();

        event EventHandler? OnOpen;

        event EventHandler? OnMessage;

        event EventHandler? OnError;

        event EventHandler? OnClose;

    }
}
