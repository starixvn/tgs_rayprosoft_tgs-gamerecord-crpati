using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.CrPati.Common;
using Tgs.GameRecord.CrPati.Core.ApplicationServices;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.CrPati.Core;

internal class ScheduleFormDispatcher : IScheduleFormDispatcher
{
    private readonly ILogger<ScheduleFormDispatcher> _logger;
    private readonly IGameRoundsScheduleClient _client;
    private readonly IScheduleFormLock _scheduleFormLock;
    private readonly ICommandService<PullRecordCommand> _pullRecorService;

    public ScheduleFormDispatcher(
        ILogger<ScheduleFormDispatcher> logger,
        IGameRoundsScheduleClient client,
        IScheduleFormLock scheduleFormLock,
        ICommandService<PullRecordCommand> pullRecorService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _scheduleFormLock = scheduleFormLock ?? throw new ArgumentNullException(nameof(scheduleFormLock));
        _pullRecorService = pullRecorService ?? throw new ArgumentNullException(nameof(pullRecorService));
    }

    public async ValueTask ExecuteAsync(
        string gameProviderCode,
        long id,
        CancellationToken cancellationToken = default)
    {
        var scheduleForm = await _client
             .ScheduleFormGetAsync(
                 gameProviderCode: gameProviderCode,
                 id: id,
                 cancellationToken: cancellationToken)
             .ConfigureAwait(false)
             ?? throw new Exception($"The game provider ({gameProviderCode}) cannot find the schedule form. ({id})");

        if (scheduleForm.State != ScheduleFormState.Queued)
        {
            _logger.LogWarning("This game provider ({gameProviderCode}) schedule form status is not queued.({id})", gameProviderCode, id);
            return;
        }

        if (!await _scheduleFormLock
            .LockingAsync(
                id: id)
            .ConfigureAwait(false))
        {
            _logger.LogWarning("LogOn:{logOn} Message:This game provider ({gameProviderCode}) schedule form has been locked. ({id})",
                DateTime.Now.ToString("o"),
                gameProviderCode,
                id);
            return;
        }

        try
        {
            await _client
                .ScheduleFormProcessingAsync(
                    gameProviderCode: gameProviderCode,
                    id: id,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var parameter = JsonSerializer.Deserialize<GameRecordMessage>(scheduleForm.Parameters)
                ?? throw new Exception($"This game provider ({gameProviderCode}) scheduling form parameter is null. ({id})");

            var command = new PullRecordCommand(
                ApiUrl: parameter.ApiUrl,
                ApiKey: parameter.ApiKey,
                ApiSecret: parameter.ApiSecret,
                StartTime: parameter.StartTime,
                EndTime: parameter.EndTime);

            _logger.LogDebug("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

            await _pullRecorService.ExecuteAsync(
                command: command,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await _client.ScheduleFormCompleteAsync(
                gameProviderCode: gameProviderCode,
                id: id,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in the game provider ({gameProviderCode}) schedule form. ({id})", gameProviderCode, id);

            await _client.ScheduleFormFaileAsync(
                gameProviderCode: gameProviderCode,
                id: id,
                status: ex.Message,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _ = await _scheduleFormLock.UnlockAsync(
                id: id)
                .ConfigureAwait(false);
        }
    }
}
