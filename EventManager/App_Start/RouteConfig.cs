using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EventManager
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
               name: "EventsCreate",
               url: "Events/{userId}/Create",
               defaults: new { controller = "Events", action = "Create" }
               );

            routes.MapRoute(
                name: "EventsByPage",
                url: "Events/{userId}/Page{pageUserEvents}",
                defaults: new
                { controller = "Events", action = "IndexById" }
                );

            routes.MapRoute(
                name: "EventsbyCategory",
                url: "Events/{userId}/{eventTypeId}",
                defaults: new { controller = "Events", action = "IndexById", eventTypeId = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
