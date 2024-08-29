using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameInterface.CrPati.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.CrPati.Core.ApplicationServices;

internal class PullRecordCommandService(
	ILogger<PullRecordCommandService> logger,
	IGameInterfaceCrPatiClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		var pageNumber = 1;

		IEnumerable<GameWagerInfo> gameWagerInfos;
		do
		{
			gameWagerInfos = await client.GameWagerAsync(
				apiUrl: command.ApiUrl,
				apiKey: command.ApiKey,
				apiSecret: command.ApiSecret,
				startTime: command.StartTime,
				endTime: command.EndTime,
				pageNumber: pageNumber,
				pageSize: 1000,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameWagerInfo>();

			if (gameWagerInfos.Any())
			{
				foreach (var gameWagerInfo in gameWagerInfos)
				{
					try
					{
						await service.ExecuteAsync(
							command: new(gameWagerInfo),
							cancellationToken: cancellationToken)
							.ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "{logError}",
							JsonSerializer.Serialize(new
							{
								LogOn = DateTime.Now.ToString("u"),
								Data = new
								{
									gameWagerInfo.PlayerCode,
									gameWagerInfo.ExternalWagerId,
									ExternalCreatedOn = gameWagerInfo.ExternalCreatedOn.ToDateTimeOffset().ToString("u"),
									ExternalClosedOn = gameWagerInfo.ExternalClosedOn.ToDateTimeOffset().ToString("u")
								},
								Message = "An exception occurred in this game wager."
							}));
					}
				}

				pageNumber++;
			}
		} while (gameWagerInfos.Any());
	}
}
