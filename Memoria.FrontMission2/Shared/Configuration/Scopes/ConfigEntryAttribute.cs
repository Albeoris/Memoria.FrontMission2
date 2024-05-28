using System;
using BepInEx.Configuration;

namespace Memoria.FrontMission2.Configuration;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ConfigEntryAttribute : Attribute
{
    public String Description { get; }
    
    public ConfigEntryAttribute(String description)
    {
        Description = description;
    }
}