using System;
using HarmonyLib;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Shared.Framework.Unity;
using TMPro;
using UnityEngine;

namespace Memoria.FrontMission2.HarmonyHooks.Battle;

// AP label fix in combat
[HarmonyPatch(typeof(Cursor), "Start")]
public static class Cursor_Start
{
    private static void Postfix(ref GameObject ___apCanvas)
    {
        if (!ModComponent.Instance.Config.Localization.ReduceBlackOutlines)
            return;
        
        try
        {
            GameObject apObject = ___apCanvas.GetChildByName("Ap");
            TextMeshProUGUI label = apObject.GetExactComponent<TextMeshProUGUI>();

            label.outlineColor = Color.white;
            label.outlineWidth = 0.07f;

            label.fontStyle = FontStyles.Italic;
            label.enableAutoSizing = false;
            label.fontSize = 34;

            Vector3 tmpVector3 = label.transform.position;
            tmpVector3.y += 1.56f;
            label.transform.position = tmpVector3;
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}