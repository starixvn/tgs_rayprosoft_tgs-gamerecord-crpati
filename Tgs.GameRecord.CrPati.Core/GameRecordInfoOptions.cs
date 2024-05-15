namespace Tgs.GameRecord.CrPati.Core;

public record GameRecordInfoOptions
{
	public string GameProviderCode { get; set; } = default!;

	public string GameExternalId { get; set; } = default!;

	public string GameProviderConfiguration { get; init; } = default!;
	public string HangfirePrefix { get; set; } = "gamerecord-cp-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-cp-locking:";
	public double IntervalMinutes { get; set; } = 10;
	public double IntervalMinutesMax { get; set; } = 120;
	public double IntervalMinutesMinimum { get; set; } = 1;
}
