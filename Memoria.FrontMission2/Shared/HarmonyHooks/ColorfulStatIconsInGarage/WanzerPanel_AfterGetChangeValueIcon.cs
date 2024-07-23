using System;
using System.Collections.Generic;
using System.Reflection;
using AdditionalPanels;
using HarmonyLib;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria.FrontMission2.HarmonyHooks.ColorfulStatIconsInGarage;

[HarmonyPatch]
public class WanzerPanel_AfterGetChangeValueIcon
{
    [HarmonyTargetMethods]
    private static IEnumerable<MethodInfo> GetTargetMethods()
    {
	    return typeof(WanzerPanel).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }

    [HarmonyPostfix]
    public static void Postfix(
		Image ___totalIcon,
		Image ___fightIcon,
		Image ___shortIcon,
		Image ___longIcon,
		Image ___moveIcon,
		Image ___mobileIcon,
		Image ___runningCostIcon,
		Image ___healthPointsIcon,
		Image ___weightPowerIcon)
    {
	    try
	    {
		    Image[] icons =
		    [
			    ___totalIcon,
			    ___fightIcon,
			    ___shortIcon,
			    ___longIcon,
			    ___moveIcon,
			    ___mobileIcon,
			    ___runningCostIcon,
			    ___healthPointsIcon,
			    ___weightPowerIcon
		    ];

		    foreach (Image icon in icons)
		    {
			    icon.color = (icon.sprite?.name) switch
			    {
				    "LowerValuesIcon" => icon.color = Color.red,
				    "HigherValuesIcon" => Color.green,
				    "SameValuesIcon" => Color.clear,
				    _ => Color.white
			    };
		    }
	    }
	    catch (Exception ex)
	    {
		    ModComponent.Log.LogException(ex);
	    }
    }
}