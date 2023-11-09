namespace Tgs.GameRecord.CrPati.ViewModel;

public class PullRecordViewModel
{
    public Uri ApiUrl { get; init; } = default!;

    public string ApiKey { get; init; } = default!;

    public string ApiSecret { get; init; } = default!;

    public DateTimeOffset StartTime { get; init; } = default!;

    public DateTimeOffset EndTime { get; init; } = default!;
}
