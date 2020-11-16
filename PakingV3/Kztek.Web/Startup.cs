using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Kztek.Web.Startup))]

namespace Kztek.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Enables SignalR
            //app.MapSignalR();
            //GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(40);
            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);
            //GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);
        }
    }
}
