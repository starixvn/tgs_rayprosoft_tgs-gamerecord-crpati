using Microsoft.Extensions.Logging;
using Tgs.GameInterface.CrPati.Client;
using Tgs.GameInterface.CrPati.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.CrPati.Core.ApplicationServices;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly IGameInterfaceCrPatiClient _client;
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        IGameInterfaceCrPatiClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var pageNumber = 1;

        IEnumerable<GameWagerInfo> gameWagerInfos;
        do
        {
            gameWagerInfos = await _client.GameWagerAsync(
                apiUrl: command.ApiUrl,
                apiKey: command.ApiKey,
                apiSecret: command.ApiSecret,
                startTime: command.StartTime,
                endTime: command.StartTime,
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
                    _logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameWagerInfo);

                    try
                    {
                        await _service.ExecuteAsync(
                            command: new(gameWagerInfo),
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameWagerInfo})", DateTime.Now.ToString("o"), gameWagerInfo);
                    }
                }
            }
        } while (gameWagerInfos.Any());
    }
}
