using System;
using HarmonyLib;
using Memoria.FrontMission2.Configuration;

namespace Memoria.FrontMission2.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(RenameHandler), "Submit", argumentTypes: [])]
public static class RenameHandler_Submit
{
    [HarmonyPrefix]
    public static void SubmitPrefix(
        out Boolean __state,
        Boolean ___isCallsignRename)
    {
        __state = !___isCallsignRename;
    }

    [HarmonyPostfix]
    public static void SubmitPostfix(
        Boolean __state,
        RenameKeyboardInput ___renameKeyboardInput)
    {
        if (__state && ModComponent.Instance.Config.Localization.ProhibitsChangingCharacterCallSigns)
        {
            ModComponent.Log.LogInfo($"Call sign changing skipped because {nameof(LocalizationConfiguration.ProhibitsChangingCharacterCallSigns)} option is enabled.");
            ___renameKeyboardInput.Submit();
        }
    }
}