using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DoodleThings
{
    using AppFunc = System.Func<IDictionary<string, object>, Task>;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapHubs("/signalr", new HubConfiguration(), AddAuth);

            IdentityConfig.ConfigureIdentity();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void AddAuth(IAppBuilder app)
        {
            app.Use(typeof(BearerMiddleware));
        }

    }

    public class BearerMiddleware
    {
        private readonly AppFunc _next;
        public BearerMiddleware(AppFunc next)
        {
            _next = next;
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            var owinRequest = new OwinRequest(env);
            var query = owinRequest.GetQuery();
            string[] accessTokens;
            if (query.TryGetValue("token", out accessTokens))
            {
                var ticket = IdentityConfig.Bearer.AccessTokenHandler.Unprotect(accessTokens[0]);
                env["server.User"] = new ClaimsPrincipal(ticket.Identity);
            }

            return _next(env);
        }
    }
}
