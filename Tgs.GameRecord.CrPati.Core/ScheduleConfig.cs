namespace Tgs.GameRecord.CrPati.Core;

public record ScheduleConfig
{
	public DateTimeOffset NextTime { get; set; }
	public string NextExternalWagerId { get; set; } = default!;
}
