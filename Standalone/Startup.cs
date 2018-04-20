using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Standalone.Startup))]
namespace Standalone
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
