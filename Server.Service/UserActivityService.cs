using AutoMapper;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IRepositories;
using Server.Core.IServices;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Server.Service
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IDataContext _context;

        public UserActivityService(IDataContext context)
        {
            _context = context;
        }

        public async Task<int> GetDailyEntryCount()
        {
            var today = DateTime.Now.Date;  // יום נוכחי ללא שעה
            var tomorrow = today.AddDays(1); // תחילת יום חדש

            return await _context.UserActivities
                .CountAsync(x => x.Timestamp >= today && x.Timestamp < tomorrow && x.ActionType == "Login");
        }


        public async Task<int> GetWeeklyEntryCount()
        {
            var weekAgo = DateTime.Now.AddDays(-7);
            return await _context.UserActivities
                .CountAsync(x => x.Timestamp >= weekAgo && x.ActionType == "Login");
        }

        public async Task<List<UserActivityPoint>> GetUserActivityData()
        {
            var now = DateTime.Now;
            var weekAgo = now.AddDays(-7);

            return await _context.UserActivities
                .Where(x => x.Timestamp >= weekAgo)
                .GroupBy(x => x.Timestamp.Date)
                .Select(g => new UserActivityPoint
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetActionFrequency()
        {
            return await _context.UserActivities
                .GroupBy(x => x.ActionType)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<List<PopularPage>> GetPopularPages()
        {
            return await _context.UserActivities
                .GroupBy(x => x.PagePath)
                .Select(g => new PopularPage
                {
                    Path = g.Key,
                    Views = g.Count()
                })
                .OrderByDescending(x => x.Views)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<HourlyActivity>> GetHourlyActivity()
        {
            return await _context.UserActivities
                .GroupBy(x => x.Timestamp.Hour)
                .Select(g => new HourlyActivity
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }


        public async Task<int> GetTotalUserCount()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetNewUsersTodayCount()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Users
                .CountAsync(u => u.CreatedAt >= today && u.CreatedAt < tomorrow);
        }


    }
}
