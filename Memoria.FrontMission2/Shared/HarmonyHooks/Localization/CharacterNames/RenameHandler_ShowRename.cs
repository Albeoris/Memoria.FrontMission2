using System;
using HarmonyLib;
using Memoria.FrontMission2.Configuration;

namespace Memoria.FrontMission2.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(RenameHandler), "ShowRename")]
public static class RenameHandler_ShowRename
{
    [HarmonyPrefix]
    public static void ShowRenamePrefix(ActorObject actorObject, ref String startingCallsign)
    {
        var option = ModComponent.Instance.Config.Localization.UseCharacterNamesInsteadCallSigns;
        if (option == LocalizationConfiguration.CallSignsFromNames.Disabled)
            return;

        String actorName = actorObject.actor.LocalizedName;
        if (option == LocalizationConfiguration.CallSignsFromNames.UpperCase)
            actorName = actorName.ToUpper();

        ModComponent.Log.LogInfo($"Default call sign changed from [{startingCallsign}] to [{actorName}] because {nameof(LocalizationConfiguration.UseCharacterNamesInsteadCallSigns)} option is set to {option}.");
        startingCallsign = actorName;
    }

    [HarmonyPostfix]
    public static void ShowRenamePostfix(
        Boolean ___isCallsignRename,
        RenameKeyboardInput ___renameKeyboardInput)
    {
        if (ModComponent.Instance.Config.Localization.ProhibitsChangingCharacterNames)
        {
            ModComponent.Log.LogInfo($"Character renaming skipped because {nameof(LocalizationConfiguration.ProhibitsChangingCharacterNames)} option is enabled.");
            ___renameKeyboardInput.Submit();
        }

        if (ModComponent.Instance.Config.Localization.ProhibitsChangingCharacterCallSigns && ___isCallsignRename)
        {
            ModComponent.Log.LogInfo($"Call sign changing skipped because {nameof(LocalizationConfiguration.ProhibitsChangingCharacterCallSigns)} option is enabled.");
            ___renameKeyboardInput.Submit();
        }
    }
}