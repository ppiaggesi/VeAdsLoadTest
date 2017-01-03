using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PerformanceDisplay.Startup))]
namespace PerformanceDisplay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
