SquishIt lets you squish some JavaScript and CSS.

This fork runs with the cache direction Justin had going on in his [development branch][1]. Writing a physical file works great as long as you have full control of the setup on the server (IIS) - the application requires write access, and you'll also need to set up the cache http headers for the folder being written to; we don't. So writing directly to memory gives us the flexibility we need. Of course, this means you'll have to take care of satisfying the requests for the files in some other fashion. This could be a custom HttpHandler, or a defined route. Since we're all ASP.NET MVC here, the routing path works great and is the focus of this fork.

So now for an example of all pieces working together...

### Create your bundles. 
Here, we're priming them as named bundles in Global.asax:

    Bundle.JavaScript()
        .Add("~/Content/fullcalendar/fullcalendar.js")
        .Add("~/Scripts/FeedManager.js")
        .Add("~/Scripts/ActionHandlers.js")
        .Add("~/Scripts/DataMethods.js")
        .Add("~/Scripts/parse.js")
        .Add("~/Scripts/jquery.periodicalupdater.js")
        .Add("~/Scripts/DateParser.js")
        .Add("~/Scripts/helpers.js")
        .AsNamedCache(ViewContent.JavaScript.Common, "common_#.js");

    Bundle.Css()
        .Add("~/Content/fullcalendar/fullcalendar.css")
        .Add("~/Content/workflow.css")
        .AsNamedCache(ViewContent.Css.Viewer, "viewer_#.css");

and rendering the named bundles in the view:

    <%=Bundle.JavaScript().RenderNamed(ViewContent.JavaScript.Common)%>
    ...
    <%=Bundle.Css().RenderNamed(ViewContent.Css.Viewer)%>

### Create the controller to handle requests
*Note: default route is "bundle/script/" and "bundle/style/" for Javascript and Css respectively, but can be overridden with SetCacheRoute()*

	public class BundleController : Controller
    {
        //
        // GET: /Bundle/Style

        [OutputCache(Duration = int.MaxValue, VaryByParam = "key")]
        public ContentResult Style(string key)
        {
            var output = Bundle.Css().GetCachedContent(key);
            return new ContentResult { Content = output, ContentType = "text/css" };
        }

        //
        // GET: /Bundle/Script

        [OutputCache(Duration = int.MaxValue, VaryByParam = "key")]
        public ContentResult Script(string key)
        {
            var output = Bundle.JavaScript().GetCachedContent(key);
            return new ContentResult { Content = output, ContentType = "text/javascript" };
        }

    }
	
### And finally, map the routes to our controller

    routes.MapRoute("SquishItStyle", "bundle/style/{key}", new
    {
        controller = "Bundle",
        action = "Style"
    });
	
    routes.MapRoute("SquishItScript", "bundle/script/{key}", new
    {
        controller = "Bundle",
        action = "Script"
    });
	
[1]: http://github.com/jetheredge/SquishIt/tree/development