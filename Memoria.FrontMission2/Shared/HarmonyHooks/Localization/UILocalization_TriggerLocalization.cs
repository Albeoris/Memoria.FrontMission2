using System;
using System.Reflection;
using HarmonyLib;
using Memoria.FrontMission2.BeepInEx;

namespace Memoria.FrontMission2.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(UILocalization), "TriggerLocalization", argumentTypes: [])]
public static class UILocalization_TriggerLocalization
{
    public static readonly MethodInfo IgnoreLocalizationSetter = AccessTools.PropertySetter(typeof(UILocalization), "IgnoreLocalization");

    public static Boolean Prefix(UILocalization __instance)
    {
        try
        {
            if (__instance.IgnoreLocalization && !__instance.IsPlaceholder)
            {
                if (ModComponent.Instance.Config.Localization.ForceLocalization)
                {
                    IgnoreLocalizationSetter.Invoke(__instance, [false]);
                    __instance.TriggerLocalization();
                    return false; // skip original
                }
            }
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }

        return true; // call original
    }
}