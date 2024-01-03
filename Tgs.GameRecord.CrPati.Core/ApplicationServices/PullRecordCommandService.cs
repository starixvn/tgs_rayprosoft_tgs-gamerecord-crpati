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
                    logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameWagerInfo);

                    try
                    {
                        await service.ExecuteAsync(
                            command: new(gameWagerInfo),
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameWagerInfo})", DateTime.Now.ToString("o"), gameWagerInfo);
                    }
                }
                pageNumber++;
            }
        } while (gameWagerInfos.Any());
    }
}
