using Microsoft.AspNetCore.Mvc;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IServices;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivityController : Controller
    {

        private readonly IUserActivityService _analyticsService;

        public UserActivityController(IUserActivityService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("daily-entries")]
        public async Task<IActionResult> GetDailyEntries() =>
            Ok(await _analyticsService.GetDailyEntryCount());

        [HttpGet("weekly-entries")]
        public async Task<IActionResult> GetWeeklyEntries() =>
            Ok(await _analyticsService.GetWeeklyEntryCount());

        [HttpGet("user-activity")]
        public async Task<IActionResult> GetUserActivity() =>
            Ok(await _analyticsService.GetUserActivityData());

        [HttpGet("action-frequency")]
        public async Task<IActionResult> GetActionFrequency() =>
            Ok(await _analyticsService.GetActionFrequency());

        [HttpGet("popular-pages")]
        public async Task<IActionResult> GetPopularPages() =>
            Ok(await _analyticsService.GetPopularPages());

        [HttpGet("hourly-activity")]
        public async Task<IActionResult> GetHourlyActivity() =>
            Ok(await _analyticsService.GetHourlyActivity());
        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers() =>
    Ok(await _analyticsService.GetTotalUserCount());

        [HttpGet("new-users-today")]
        public async Task<IActionResult> GetNewUsersToday() =>
            Ok(await _analyticsService.GetNewUsersTodayCount());

    }
}
