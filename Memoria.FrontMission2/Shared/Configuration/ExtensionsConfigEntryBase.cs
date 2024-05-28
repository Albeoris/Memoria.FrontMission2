using System;
using BepInEx.Configuration;
using Memoria.FrontMission2.HarmonyHooks;

namespace Memoria.FrontMission2.Configuration;

public static class ExtensionsConfigEntryBase
{
    public static Boolean HasFileDefinedValue(this ConfigEntryBase entry)
    {
        return entry.Description is MemoriaConfigDescription { HasFileDefinedValue: true };
    }
}