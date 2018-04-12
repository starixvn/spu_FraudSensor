[assembly: Microsoft.Owin.OwinStartupAttribute(typeof(Fanex.NAP.FS.Websites.Startup))]

namespace Fanex.NAP.FS.Websites
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}