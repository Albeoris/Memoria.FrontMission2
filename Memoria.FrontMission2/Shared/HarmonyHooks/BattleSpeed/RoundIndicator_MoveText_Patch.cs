// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection.Emit;
// using HarmonyLib;
//
// namespace Memoria.FrontMission2.HarmonyHooks.BattleSpeed;
//
// // Speedup ui turn animations in battle
// [HarmonyPatch(typeof(RoundIndicator), "MoveText")]
// public static class RoundIndicator_MoveText_Patch
// {
//     static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//     {
//         List<CodeInstruction> codes = new(instructions);
//
//         for (Int32 i = 0; i < codes.Count; i++)
//         {
//             if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i + 1].opcode == OpCodes.Stfld)
//             {
//                 if (codes[i + 1].operand.ToString().Contains("moveDuration"))
//                 {
//                     codes[i].operand = 0.5f;
//
//                     break;
//                 }
//             }
//         }
//
//         for (Int32 i = 0; i < codes.Count; i++)
//         {
//             if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i + 1].opcode == OpCodes.Stfld)
//             {
//                 if (codes[i + 1].operand.ToString().Contains("pauseDuration"))
//                 {
//                     codes[i].operand = 0.25f;
//
//                     break;
//                 }
//             }
//         }
//
//         return codes.AsEnumerable();
//     }
// }