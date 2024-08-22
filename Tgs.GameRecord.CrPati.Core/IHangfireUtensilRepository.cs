using Tgs.GameRecord.CrPati.Core.JobApplicationServices;

namespace Tgs.GameRecord.CrPati.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
