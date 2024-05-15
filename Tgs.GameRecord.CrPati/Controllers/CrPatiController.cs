using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.CrPati.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.CrPati.Controllers;

[Route("[controller]")]
[ApiController]
public class CrPatiController : ControllerBase
{
	/// <summary>
	///
	/// </summary>
	/// <param name="messageSender"></param>
	/// <param name="source"></param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "https://apitest.fastapi2020.com/",
	///             "apiKey": "testgpi123123",
	///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
	///             "startTime": "2023-11-09T10:00:00+08:00",
	///             "endTime": "2023-11-09T11:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost]
	public ValueTask GameRecordAsync(
	   [FromServices] IMessageSender messageSender,
	   CrPatiGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: CrPatiNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
