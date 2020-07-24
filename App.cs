using System;
using System.Collections.Generic;
using System.Text;
using HuajiTech.CoolQ.Events;

namespace Cat_Robot
{
    internal static class App
    {
        public static void Init()
        {
            IBotEventSource botEventSource = BotEventSource.Instance;
            ICurrentUserEventSource currentUserEventSource = CurrentUserEventSource.Instance;
            IGroupEventSource groupEventSource = GroupEventSource.Instance;

            new Main(currentUserEventSource);
        }
    }
}