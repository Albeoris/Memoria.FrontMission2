using System;
using System.Reflection;
using HarmonyLib;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;

namespace Memoria.FrontMission2.HarmonyHooks.AttackRange;

[HarmonyPatch(typeof(Tile), nameof(Tile.ChangeHighlight))]
public static class Tile_ChangeHighlight
{
    private static readonly FastInvokeHandler _setTileColor;

    delegate void SetTileColorDelegate(Color color);

    static Tile_ChangeHighlight()
    {
        MethodInfo setTileColorMethod = AccessTools.Method(typeof(Tile), "SetTileColor");
        _setTileColor = MethodInvoker.GetHandler(setTileColorMethod);
    }
    
    private static Boolean Prefix(HighlightType highlightType,
        Tile __instance,
        MeshRenderer ___highlightRenderer)
    {
        try
        {
            UInt32 value = (UInt32)highlightType;
            if (value < 10)
                return true; // call original

            if (___highlightRenderer != null)
                ___highlightRenderer.enabled = true;

            Color color = ColorHelper.DecodeColor(value);
            _setTileColor(__instance, color);
            return false; // skip original
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
            return false; // skip original
        }
    }
}