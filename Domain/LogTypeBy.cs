using Thinktecture;

namespace DatabaseEasyLog.Domain;

public sealed partial class LogTypeBy : IEnum<string>
{
    public static readonly LogTypeBy User = new("User");
    public static readonly LogTypeBy System = new("System");
}