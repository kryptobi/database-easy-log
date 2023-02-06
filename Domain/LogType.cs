using Thinktecture;

namespace DatabaseEasyLog.Domain;

public sealed partial class LogType : IEnum<string>
{
    public static readonly LogType Added = new("Added");
    public static readonly LogType Modified = new("Modified");
    public static readonly LogType Deleted = new("Deleted");
}