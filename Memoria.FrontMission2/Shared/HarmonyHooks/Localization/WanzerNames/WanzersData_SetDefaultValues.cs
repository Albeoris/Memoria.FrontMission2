using System;
using System.Linq;
using HarmonyLib;
using I2.Loc;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Configuration;

namespace Memoria.FrontMission2.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(WanzersData), "SetDefaultValues", argumentTypes: [])]
public static class WanzersData_SetDefaultValues
{

    [HarmonyPostfix]
    public static void SetDefaultValuesPostfix(WanzersData __instance)
    {
        try
        {
            TryLocalizeWanzerName(nameof(WanzersData.Bounder), ref __instance.Bounder);
            TryLocalizeWanzerName(nameof(WanzersData.DuskMan), ref __instance.DuskMan);
            TryLocalizeWanzerName(nameof(WanzersData.CuteBird), ref __instance.CuteBird);
            TryLocalizeWanzerName(nameof(WanzersData.Kasumi), ref __instance.Kasumi);
            TryLocalizeWanzerName(nameof(WanzersData.AceJoker), ref __instance.AceJoker);
            TryLocalizeWanzerName(nameof(WanzersData.JackArms), ref __instance.JackArms);
            TryLocalizeWanzerName(nameof(WanzersData.Bilancia), ref __instance.Bilancia);
            TryLocalizeWanzerName(nameof(WanzersData.Topaz), ref __instance.Topaz);
            TryLocalizeWanzerName(nameof(WanzersData.BoldCat), ref __instance.BoldCat);
            TryLocalizeWanzerName(nameof(WanzersData.Durabler), ref __instance.Durabler);
            TryLocalizeWanzerName(nameof(WanzersData.Basurero), ref __instance.Basurero);
            TryLocalizeWanzerName(nameof(WanzersData.Nouko), ref __instance.Nouko);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void TryLocalizeWanzerName(String localizationKey, ref String wanzerName)
    {
        var fullKey = $"HARDCODED/DEFAULT_WANZER_NAMES/{localizationKey}";
        if (LocalizationManager.TryGetTranslation(fullKey, out String result, applyParameters: true))
            wanzerName = result;
    }
}