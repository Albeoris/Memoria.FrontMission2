using System;
using HarmonyLib;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Shared.Framework.UIChanger;

namespace Memoria.FrontMission2.HarmonyHooks;

[HarmonyPatch(typeof(PartButtonGarage), "Awake")]
public class PartButtonGarage_Awake
{
    public static Boolean HasErrors { get; private set; }
    
    private static void Postfix(PartButtonGarage __instance)
    {
        try
        {
            if (ModComponent.Instance.Config.UI.GarageEquipWeaponPanelWidth < 1)
                return;
            
            if (!StretchableGarageEquipmentListEntry.TryMakeStretchable(__instance, out _, out FormattableString reason))
            {
                ModComponent.Log.LogError($"[{nameof(WeaponButtonGarage_Awake)}].{nameof(Postfix)}(): Cannot stretch [{__instance.name}]. Reason: {reason}");
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