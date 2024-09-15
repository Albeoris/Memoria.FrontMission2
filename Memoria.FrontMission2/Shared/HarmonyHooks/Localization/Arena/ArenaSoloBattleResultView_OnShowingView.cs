using System;
using System.Reflection;
using Colosseum;
using HarmonyLib;
using I2.Loc;
using Memoria.FrontMission2.BeepInEx;

namespace Memoria.FrontMission2.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(ArenaSoloBattleResultView), "OnShowingView", argumentTypes: [])]
public static class ArenaSoloBattleResultView_OnShowingView
{
    public static void Prefix(ArenaSoloBattleResultView __instance,
        ref String ___bonusDescription,
        ref String ___excellentDescription,
        ref String ___looseDescription,
        ref String ___drawDescription,
        ref String ___wonDescription)
    {
        try
        {
            TryLocalize("BONUS", ref ___bonusDescription);
            TryLocalize("EXCELLENT", ref ___excellentDescription);
            TryLocalize("LOOSE", ref ___looseDescription);
            TryLocalize("DRAW", ref ___drawDescription);
            TryLocalize("WON", ref ___wonDescription);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void TryLocalize(String localizationKey, ref String text)
    {
        String fullKey = $"HARDCODED/ARENA_SOLO/{localizationKey}";
        if (LocalizationManager.TryGetTranslation(fullKey, out String result, applyParameters: true))
            text = result;
    }
}