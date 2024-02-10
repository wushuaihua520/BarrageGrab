using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.GrabServices
{
    /// <summary>
    /// 弹幕抓取服务规范接口
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
