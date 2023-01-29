using Thinktecture;

namespace DbLogger;

public sealed partial class LogTypeBy : IEnum<string>
{
    public static readonly LogTypeBy User = new("User");
    public static readonly LogTypeBy System = new("System");
}