using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(travel_agency.Startup))]
namespace travel_agency
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
