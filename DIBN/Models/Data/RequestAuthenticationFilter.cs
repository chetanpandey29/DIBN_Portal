using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DIBN.Models.Data
{
    [Authorize]
    public class RequestAuthenticationFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RequestAuthenticationFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="context"></param>
        /// 
        [Authorize]
        public void OnActionExecuting(ActionExecutingContext context)
        {
                var session = _httpContextAccessor.HttpContext.Session.Get("UserEmail");
                if (context.HttpContext.Session == null || !context.HttpContext.Session.TryGetValue("Id", out byte[] val))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Account" }));
                }
            
        }

        /// <summary>
        /// OnActionExecuted
        /// </summary>
        /// <param name="context"></param>
        [Authorize]
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
