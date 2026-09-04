using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SlojServisa
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Postavljanje UTF-8 enkodiranja za JSON formatter
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SupportedEncodings.Clear();
            jsonFormatter.SupportedEncodings.Add(System.Text.Encoding.UTF8);

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
