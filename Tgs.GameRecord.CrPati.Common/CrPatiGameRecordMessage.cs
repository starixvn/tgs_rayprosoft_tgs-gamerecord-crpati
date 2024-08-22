using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.CrPati.Common;

public record CrPatiGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://apitest.fastapi2020.com/")]
	public Uri ApiUrl { get; init; } = default!;

	[Required]
	[DefaultValue(typeof(string), "testgpi123123")]
	public string ApiKey { get; init; } = default!;

	[Required]
	[DefaultValue(typeof(string), "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR")]
	public string ApiSecret { get; init; } = default!;

	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-11-09T10:00:00+08:00")]
	public DateTimeOffset StartTime { get; init; } = default!;

	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-11-09T11:00:00+08:00")]
	public DateTimeOffset EndTime { get; init; } = default!;
}
