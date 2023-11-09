using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.CrPati.Common;
using Tgs.GameRecord.CrPati.Core;
using Tgs.GameRecord.CrPati.ViewModel;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Protos;

namespace Tgs.GameRecord.CrPati.Controllers;

[Route("[controller]")]
[ApiController]
public class CrPatiController : ControllerBase
{
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public CrPatiController(IOptions<GameRecordInfoOptions> options)
    {
        _gameRecordInfo = options.Value;
    }

    /// <summary>
    /// Games the record asynchronous.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="source">The source.</param>
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
       [FromServices] IGameRoundsScheduleClient client,
       [FromBody] PullRecordViewModel source)
    {
        var parameters = new GameRecordMessage
        {
            ApiUrl = source.ApiUrl,
            ApiKey = source.ApiKey,
            ApiSecret = source.ApiSecret,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
        };

        return client.ScheduleFormQueuedAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            name: source.ApiKey,
            subject: "gp.gamerecord.scheduleform.cp.queued",
            source: ScheduleFormSource.Schedule,
            parameters: JsonSerializer.Serialize(parameters),
            cancellationToken: HttpContext.RequestAborted);
    }
}
