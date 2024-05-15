using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.CrPati.Protos;
using Tgs.GameRecord.CrPati.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.CrPati.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task BetState_Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "CP",
			GameProviderConfiguration = "CPConfiguration",
			GameExternalId = "lottery",
			HangfirePrefix = "gamerecord-cp-hangfire:",
			HangfireLocking = "gamerecord-cp-hangfire:",
			IntervalMinutes = 10,
			IntervalMinutesMax = 120,
			IntervalMinutesMinimum = 1
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordCommand(
			Info: new GameWagerInfo
			{
				ExternalWagerId = "231108-f2df556f48",
				PlayerCode = "4ue0ujotpmuu",
				IsClosed = true,
				IsVoid = false,
				IsCancel = false,
				IsCashout = false,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-11-08T15:55:33.22+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-11-08T16:20:02.18+08:00").ToTimestamp(),
				Bet = 2.0m,
				ValidBet = 2.0m,
				Settle = 0,
				WinLoss = -2.0m,
				AdditionalData = new JsonObject
				{
					["BetID"] = "231108-f2df556f48",
					["Odds"] = 0.0,
					["OddsType"] = 0,
					["GameCode"] = "",
					["MemberAccount"] = "4ue0ujotpmuu",
					["ModifyDate"] = "2023-11-08T16:21:31.291+08:00",
					["BetDate"] = "2023-11-08T15:55:33.22+08:00",
					["SettleDate"] = "2023-11-08T16:20:02.18+08:00",
					["Bet"] = 2.0,
					["ValidBet"] = 2.0,
					["TotalPayout"] = 0.0,
					["TotalWinlose"] = -2.0,
					["BetContent"] = "Lottery type : HighSpeedMatka20<br>Betting number : B<br>Round Code : 231108041",
					["BetStatus"] = 3,
					["GameType"] = 16,
					["GameKind"] = 0
				}.ToJsonString()
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalWagerId,
				ExternalBetId = command.Info.ExternalWagerId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn,
				ExternalClosedOn = command.Info.ExternalClosedOn,
				ExternalPayoffOn = command.Info.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 2.0m,
				ValidBet = 2.0m,
				Settle = 0.0M,
				Commission = 0.0m,
				Turnover = 2.0m,
				WinLoss = -2.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "CP",
			GameProviderConfiguration = "CPConfiguration",
			GameExternalId = "lottery",
			HangfirePrefix = "gamerecord-cp-hangfire:",
			HangfireLocking = "gamerecord-cp-hangfire:",
			IntervalMinutes = 10,
			IntervalMinutesMax = 120,
			IntervalMinutesMinimum = 1
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordCommand(
			Info: new GameWagerInfo
			{
				ExternalWagerId = "231108-31029312af",
				PlayerCode = "4ue0ujotpmuu",
				IsClosed = true,
				IsVoid = false,
				IsCancel = false,
				IsCashout = false,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-11-08T17:40:05.12+08:00").ToTimestamp(),
				Bet = 2.0m,
				ValidBet = 2.0m,
				Settle = 3.8m,
				WinLoss = 1.8m,
				AdditionalData = new JsonObject
				{
					["BetID"] = "231108-31029312af",
					["Odds"] = 0.0,
					["OddsType"] = 0,
					["GameCode"] = "",
					["MemberAccount"] = "4ue0ujotpmuu",
					["ModifyDate"] = "2023-11-08T17:42:23.225+08:00",
					["BetDate"] = "2023-11-08T17:13:01.43+08:00",
					["SettleDate"] = "2023-11-08T17:40:05.12+08:00",
					["Bet"] = 2.0,
					["ValidBet"] = 2.0,
					["TotalPayout"] = 3.8,
					["TotalWinlose"] = 1.8,
					["BetContent"] = "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045",
					["BetStatus"] = 3,
					["GameType"] = 16,
					["GameKind"] = 0
				}.ToJsonString()
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalWagerId,
				ExternalBetId = command.Info.ExternalWagerId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn,
				ExternalClosedOn = command.Info.ExternalClosedOn,
				ExternalPayoffOn = command.Info.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 2.0m,
				ValidBet = 2.0m,
				Settle = 3.8m,
				Commission = 0.0m,
				Turnover = 2.0m,
				WinLoss = 1.8m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Running()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "CP",
			GameProviderConfiguration = "CPConfiguration",
			GameExternalId = "lottery",
			HangfirePrefix = "gamerecord-cp-hangfire:",
			HangfireLocking = "gamerecord-cp-hangfire:",
			IntervalMinutes = 10,
			IntervalMinutesMax = 120,
			IntervalMinutesMinimum = 1
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordCommand(
			Info: new GameWagerInfo
			{
				ExternalWagerId = "231108-31029312af",
				PlayerCode = "4ue0ujotpmuu",
				IsClosed = false,
				IsVoid = false,
				IsCancel = false,
				IsCashout = false,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				Bet = 2.0m,
				ValidBet = 0.0m,
				Settle = 0.0m,
				WinLoss = 0.0m,
				AdditionalData = new JsonObject
				{
					["BetID"] = "231108-31029312af",
					["Odds"] = 0.0,
					["OddsType"] = 0,
					["GameCode"] = "",
					["MemberAccount"] = "4ue0ujotpmuu",
					["ModifyDate"] = "2023-11-08T17:15:08.809+08:00",
					["BetDate"] = "2023-11-08T17:13:01.43+08:00",
					["SettleDate"] = null,
					["Bet"] = 2.0,
					["ValidBet"] = 0.0,
					["TotalPayout"] = 0.0,
					["TotalWinlose"] = 0.0,
					["BetContent"] = "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045",
					["BetStatus"] = 0,
					["GameType"] = 16,
					["GameKind"] = 0
				}.ToJsonString()
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalWagerId,
				ExternalBetId = command.Info.ExternalWagerId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn,
				ExternalClosedOn = command.Info.ExternalClosedOn,
				ExternalPayoffOn = command.Info.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 2.0m,
				ValidBet = 2.0m,
				Settle = 0.0m,
				Commission = 0.0m,
				Turnover = 0.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
