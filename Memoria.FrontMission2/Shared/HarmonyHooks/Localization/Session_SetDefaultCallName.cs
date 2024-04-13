// using System;
// using HarmonyLib;
// using Memoria.FrontMission2.BeepInEx;
// using Walker;
//
// namespace Memoria.FrontMission2.HarmonyHooks;
//
// [HarmonyPatch(typeof(Session), "SetDefaultCallName")]
// public static class Session_SetDefaultCallName
// {
//     public static void Postfix(Session __instance)
//     {
//         try
//         {
//             String key = __instance.GetGameMode() == GAME_MODE.GAME_MODE_ANOTHER
//                 ? "Memoria.Hardcoded.DefaultCallName.Kevin"
//                 : "Memoria.Hardcoded.DefaultCallName.Royd";
//
//             if (LocalizationHelper.TryGet(key, out String name))
//             {
//                 ModComponent.Log.LogMessage($"Default pilot call name changed to {name}");
//                 __instance.Prefs.SetPilotCallName(0, name);
//             }
//         }
//         catch (Exception ex)
//         {
//             ModComponent.Log.LogException(ex);
//         }
//     }
// }