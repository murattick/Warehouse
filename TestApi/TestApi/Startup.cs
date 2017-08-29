using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestApi.Startup))]
namespace TestApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
