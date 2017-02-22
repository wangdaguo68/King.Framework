using Owin;

namespace King.Framework.UrlAuthorization.Owin
{
    using Owin;
    using System;
    using System.Runtime.CompilerServices;

    public static class UrlAuthorizationExtensions
    {
        public static IAppBuilder UseUrlAuthorization(this IAppBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            app.Use(typeof(UrlAuthorizationMiddleware), new object[0]);
            return app;
        }
    }
}

