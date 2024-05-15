using Hangfire.Dashboard;

namespace Tgs.GameRecord.CrPati;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
