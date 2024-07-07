using backend.Dao.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace backend.Model
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int timeToLiveSeconds;
        public CacheAttribute(int _timeToLiveSeconds)
        {
            timeToLiveSeconds = _timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
            if (!cacheConfiguration.Enabled)
            {
                await next();
                return;
            }
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKEy = GenerateCaccheKey(context.HttpContext.Request);

            var cacheResponse = await cacheService.GetCacheResponseAsync(cacheKEy);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cacheResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }
            var excutedCode = await next();
            if(excutedCode.Result is OkObjectResult objectResult)
            {
                await cacheService.SetCacheResponseAsync(cacheKEy,objectResult.Value,TimeSpan.FromSeconds(timeToLiveSeconds));  
            }

        }
        //GET /api/data?name=Dat&age=30
        public static string GenerateCaccheKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach(var (key, value) in request.Query.OrderBy(a=> a.Key)) {
                 keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString(); // /api/data|age-30|name-Dat

        }
    }
}
