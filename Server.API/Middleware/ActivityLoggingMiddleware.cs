using Server.Core.Entities;
using Server.Core.IServices;
using System.Security.Claims;
using Server.Core.IServices;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Data;

namespace Server.API.Middleware
{
    public class ActivityLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IDataContext _db;
        //private readonly IUserService _userService;

        public ActivityLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
         
        }
        public async Task Invoke(HttpContext context)
        {
            var db = context.RequestServices.GetService<IDataContext>();
            var request = context.Request;
            var originalBodyStream = context.Response.Body;
            var userEmail = context.User.Identity?.IsAuthenticated == true ?
                context.User.FindFirst(ClaimTypes.Email)?.Value
                : null;
            //var user = await _userService.GetByEmailAsync(userEmail);
          
            var activity = new UserActivity
            {
                PagePath = request.Path,
                ActionType = InferActionType(request.Path, request.Method),
                Timestamp = DateTime.Now  
            };

            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            await _next(context); // המשך לבקשה

            //activity.StatusCode = context.Response.StatusCode; // הוספת קוד סטטוס לתגובה

            db.UserActivities.Add(activity);
            await db.SaveChangesAsync();

            // החזרת גוף התגובה המקורי
            responseStream.Seek(0, SeekOrigin.Begin);
            await responseStream.CopyToAsync(originalBodyStream);
        }

        private string InferActionType(string path, string method)
        {
            if (path.Contains("/login"))
                return "Login";
            if (method == "GET") return "View";
            if (method == "POST") return "Create";
            if (method == "PUT" || method == "PATCH") return "Edit";
            if (method == "DELETE") return "Delete";
            return "Other";
        }
    }
}
