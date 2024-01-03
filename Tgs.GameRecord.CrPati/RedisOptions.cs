using Tgs.GameRecord.CrPati.Core;
namespace Tgs.GameRecord.CrPati;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
