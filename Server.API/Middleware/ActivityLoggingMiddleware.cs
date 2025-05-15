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
        private readonly IServiceScopeFactory _scopeFactory;

        public ActivityLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var scope = _scopeFactory.CreateScope()) // יצירת Scope חדש
            {
                var db = scope.ServiceProvider.GetRequiredService<IDataContext>(); // קבלת IDataContext מה-Scope

                var request = context.Request;
                var originalBodyStream = context.Response.Body;
                var userEmail = context.User.Identity?.IsAuthenticated == true ?
                    context.User.FindFirst(ClaimTypes.Email)?.Value
                    : null;

                var activity = new UserActivity
                {
                    PagePath = request.Path,
                    ActionType = InferActionType(request.Path, request.Method),
                    Timestamp = DateTime.Now
                };

                var responseStream = new MemoryStream();
                context.Response.Body = responseStream;

                await _next(context); // המשך לבקשה

                db.UserActivities.Add(activity);
                await db.SaveChangesAsync();

                responseStream.Seek(0, SeekOrigin.Begin);
                await responseStream.CopyToAsync(originalBodyStream);
            }
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
