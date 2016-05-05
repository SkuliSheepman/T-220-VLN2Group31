using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Codex.Startup))]
namespace Codex
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
