using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace xfactor
{
    [HubName("notification_Bordereau")]
    public class Notification_Bordereau : Hub
    {
        public static void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Notification_Bordereau>();
            context.Clients.All.displayNotification();
        }
    }
}