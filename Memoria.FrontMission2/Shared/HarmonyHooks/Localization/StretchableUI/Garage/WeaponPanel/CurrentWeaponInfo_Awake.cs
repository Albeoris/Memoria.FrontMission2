using System;
using HarmonyLib;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Shared.Framework.UIChanger;

namespace Memoria.FrontMission2.HarmonyHooks;

[HarmonyPatch(typeof(CurrentWeaponInfo), "Awake")]
public class CurrentWeaponInfo_Awake
{
    public static Boolean HasErrors { get; private set; }

    private static void Postfix(CurrentWeaponInfo __instance)
    {
        try
        {
            if (ModComponent.Instance.Config.UI.GarageEquipWeaponPanelWidth < 1)
                return;
            
            if (!StretchableGarageSimpleListButton.TryMakeStretchable(__instance, out _, out FormattableString reason))
            {
                ModComponent.Log.LogError($"[{nameof(CurrentWeaponInfo_Awake)}].{nameof(Postfix)}(): Cannot stretch [{__instance.name}]. Reason: {reason}");
                HasErrors = true;
            }
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
            HasErrors = true;
        }
    }
}