using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameRecord.Handle.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.CrPati.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
	IOptions<PolicySettingsOptions> policySettingsOptions,
	IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfoOptions = gameRecordInfoOptions.Value;
	private readonly PolicySettingsOptions _policySettingsOptions = policySettingsOptions.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var betState = Handle.Common.Protos.BetState.Running;
		var externalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset();
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var bet = command.Info.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = command.Info.Bet.ToDecimal().GetValueOrDefault(0);
		var settle = 0.0m;
		var turnover = 0.0m;
		var winLoss = 0.0m;
		var cashoutState = Handle.Common.Protos.CashoutState.None;

		if (command.Info.IsClosed)
		{
			externalClosedOn = command.Info.ExternalClosedOn.ToDateTimeOffset();
			settle = command.Info.Settle.ToDecimal().GetValueOrDefault(0);
			validBet = command.Info.ValidBet.ToDecimal().GetValueOrDefault(0);
			turnover = command.Info.ValidBet.ToDecimal().GetValueOrDefault(0);
			winLoss = command.Info.WinLoss.ToDecimal().GetValueOrDefault(0);
			betState = command.Info.IsVoid ? Handle.Common.Protos.BetState.Void :
				command.Info.IsCancel ? Handle.Common.Protos.BetState.Cancel :
				command.Info.IsCashout ? Handle.Common.Protos.BetState.Refund
				: winLoss <= 0 ? Handle.Common.Protos.BetState.Loss : Handle.Common.Protos.BetState.Win;
			cashoutState = command.Info.IsCashout ? Handle.Common.Protos.CashoutState.Cashout : Handle.Common.Protos.CashoutState.None;
		}

		var source = new Handle.Common.ViewModels.GameRecordHandleViewModel
		{
			GameProviderCode = _gameRecordInfoOptions.GameProviderCode,
			PlayerCode = command.Info.PlayerCode,
			ExternalRoundId = command.Info.ExternalWagerId,
			ExternalBetId = command.Info.ExternalWagerId,
			ExternalGameId = _gameRecordInfoOptions.GameExternalId,
			ExternalCreatedOn = externalCreatedOn,
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = Handle.Common.Protos.FreeGameState.None,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = cashoutState,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = command.Info.AdditionalData
		};

		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "{logWarning}",
					new
					{
						LogOn = DateTime.Now.ToString("u"),
						RetryTime = retryTime,
						RetryCount = retryCount,
						Data = new
						{
							source.GameProviderCode,
							source.PlayerCode,
							source.ExternalRoundId,
							source.ExternalBetId,
							source.ExternalGameId,
							ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
							ExternalClosedOn = source.ExternalClosedOn.ToString("u"),
							source.BetState
						},
						exception.Message
					}));

		await retryPolicy.ExecuteAsync(() => client.HandleAsync(
			source: source,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);
	}
}
