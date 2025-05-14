using Server.Core.DTOs;
using Server.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IUserActivityService
    {
        Task<int> GetDailyEntryCount();
        Task<int> GetWeeklyEntryCount();
        Task<List<UserActivityPoint>> GetUserActivityData();
        Task<Dictionary<string, int>> GetActionFrequency();
        Task<List<PopularPage>> GetPopularPages();
        Task<List<HourlyActivity>> GetHourlyActivity();
        Task<int> GetTotalUserCount();
        Task<int> GetNewUsersTodayCount();

    }

}