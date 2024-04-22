using System;
using HarmonyLib;
using I2.Loc;
using Memoria.FrontMission2.BeepInEx;

namespace Memoria.FrontMission2.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(DialogueManager), "PlayNextSentence")]
public static class DialogueManager_PlayNextSentence
{
    public static String LastMessageText { get; set; }

    public static void Postfix(
        IDialogue[] ___dialogues,
        Int32 ___dialogueIndex,
        Int32 ___sentenceIndex)
    {
        try
        {
            LastMessageText = String.Empty;
            
            if (___dialogues is null)
                return;

            if (___dialogueIndex >= ___dialogues.Length)
                return;

            SentenceField[] sentences = ___dialogues[___dialogueIndex].GetSentenceFields();
            if (___sentenceIndex >= sentences.Length)
                return;

            SentenceField sentence = sentences[___sentenceIndex];
            if (LocalizationManager.TryGetTranslation(Term: sentence.sentence,
                    Translation: out String text,
                    FixForRTL: true, maxLineLengthForRTL: 0,
                    ignoreRTLnumbers: true,
                    applyParameters: true,
                    localParametersRoot: null,
                    overrideLanguage: null,
                    allowLocalizedParameters: true))
                LastMessageText = text;
            else
                LastMessageText = String.Empty;
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}