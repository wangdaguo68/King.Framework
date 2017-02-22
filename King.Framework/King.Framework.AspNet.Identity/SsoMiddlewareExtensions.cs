namespace King.Framework.AspNet.Identity
{
    using Microsoft.Owin.Extensions;
    using Owin;
    using System;
    using System.Runtime.CompilerServices;

    public static class SsoMiddlewareExtensions
    {
        public static IAppBuilder UseKingSso(this IAppBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            app.UseStageMarker(0);
            app.Use(typeof(KingSsoMiddleware2));
            app.UseStageMarker(nameof(app));
            return app;
        }
    }
}

