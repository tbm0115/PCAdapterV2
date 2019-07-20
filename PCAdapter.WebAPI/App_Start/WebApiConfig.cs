
using System.Net.Http.Headers;
using System.Web.Http;

namespace PCAdapter.WebAPI
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
      config.MapHttpAttributeRoutes();
      //config.Routes.MapHttpRoute(
      //    name: "DefaultApi",
      //    routeTemplate: "api/{controller}/{action}", // {id}
      //    defaults: new { id = RouteParameter.Optional }
      //);
    }
  }
}
