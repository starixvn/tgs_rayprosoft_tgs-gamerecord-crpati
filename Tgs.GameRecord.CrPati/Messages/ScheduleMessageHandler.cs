using System.Text;
using System.Text.Json;
using Tgs.GameRecord.CrPati.Common;
using Tgs.GameRecord.CrPati.Core;
using Tgs.GameRecord.CrPati.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.CrPati.Messages;

internal class ScheduleMessageHandler(
   ILogger<ScheduleMessageHandler> logger,
   IPullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<CrPatiGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.ApiUrl,
					message.ApiKey,
					message.ApiSecret,
					message.StartTime,
					message.EndTime
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new PullRecordCommand(
				ApiUrl: message.ApiUrl,
				ApiKey: message.ApiKey,
				ApiSecret: message.ApiSecret,
				StartTime: message.StartTime,
				EndTime: message.EndTime),
			cancellationToken: cancellationToken);
	}
}
