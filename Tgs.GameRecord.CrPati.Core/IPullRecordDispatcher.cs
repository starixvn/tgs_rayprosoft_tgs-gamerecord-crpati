using Tgs.GameRecord.CrPati.Core.ApplicationServices;

namespace Tgs.GameRecord.CrPati.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}