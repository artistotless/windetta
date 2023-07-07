using Microsoft.AspNetCore.Mvc.Razor;

namespace Windetta.Identity.Mvc;

public static class Extensions
{
    public static void ConfigureCustomViewsRouting(this IServiceCollection services)
    {
        services.Configure<RazorViewEngineOptions>(o =>
        {
            // {2} is area, {1} is controller,{0} is the action    
            o.ViewLocationFormats.Clear();
            o.ViewLocationFormats.Add("/Mvc/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            o.ViewLocationFormats.Add("/Mvc/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
        });
    }
}
