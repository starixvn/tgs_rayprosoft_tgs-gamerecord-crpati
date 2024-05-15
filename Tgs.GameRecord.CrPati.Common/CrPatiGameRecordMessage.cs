using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.CrPati.Common;

public record CrPatiGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string ApiKey { get; init; } = default!;
	[Required]
	public string ApiSecret { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; } = default!;
}
