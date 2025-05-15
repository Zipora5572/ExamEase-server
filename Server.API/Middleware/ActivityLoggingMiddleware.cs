using Server.Core.Entities;
using System.Security.Claims;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Server.Data;

namespace Server.API.Middleware
{
    public class ActivityLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public ActivityLoggingMiddleware(RequestDelegate next, IDbContextFactory<DataContext> contextFactory)
        {
            _next = next;
            _contextFactory = contextFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var originalBodyStream = context.Response.Body;

            var activity = new UserActivity
            {
                PagePath = request.Path,
                ActionType = InferActionType(request.Path, request.Method),
                Timestamp = DateTime.Now
            };

            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            await _next(context); // המשך לבקשה

            try
            {
                using var db = _contextFactory.CreateDbContext();
                db.UserActivities.Add(activity);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // אפשר לרשום ליומן אם צריך
            }

            responseStream.Seek(0, SeekOrigin.Begin);
            await responseStream.CopyToAsync(originalBodyStream);
        }

        private string InferActionType(string path, string method)
        {
            if (path.Contains("/login")) return "Login";
            if (method == "GET") return "View";
            if (method == "POST") return "Create";
            if (method == "PUT" || method == "PATCH") return "Edit";
            if (method == "DELETE") return "Delete";
            return "Other";
        }
    }
}
