using System;

namespace DbLogger.Tests.Misc;

public class Entity
{
    public Guid Id { get; set; }
    public string Value { get; set; }

    public Entity(string value)
    {
        Id = Guid.NewGuid();
        Value = value;
    }

    public Entity SetValue(string value)
    {
        Value = value;
        return this;
    }
}