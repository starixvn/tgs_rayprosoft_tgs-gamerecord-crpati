namespace Tgs.GameRecord.CrPati.Core.ApplicationServices;

public record PullRecordCommand(
    Uri ApiUrl,
    string ApiKey,
    string ApiSecret,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
