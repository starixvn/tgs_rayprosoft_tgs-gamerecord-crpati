namespace Tgs.GameRecord.CrPati.Core.JobApplicationServices;

public record JobExecuteCommand(
    string GameProviderCode,
    string Name,
    Uri ApiUrl,
    string ApiKey,
    string ApiSecret);
