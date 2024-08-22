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
	[HttpPost]
	public ValueTask GameRecordAsync(
	   [FromServices] IMessageSender messageSender,
	   CrPatiGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: CrPatiNatsConst.ManualPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);
}
