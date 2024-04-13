using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using I2.Loc;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Configuration;
using Memoria.FrontMission2.Core;

namespace Memoria.FrontMission2.HarmonyHooks;

[HarmonyPatch(typeof(LocalizationManager), "AddSource")]
public static class LocalizationManager_AddSource
{
    public static void Prefix(ref Boolean __state)
    {
        try
        {
            __state = LocalizationManager.Sources.Count == 0;
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    public static void Postfix(LanguageSourceData Source)
    {
        try
        {
            AssetsConfiguration config = ModComponent.Instance.Config.Assets;
            if (!config.ModsEnabled)
                return;

            String assetsRoot = $"Localization/{LocalizationManager.CurrentLanguage}";
            IReadOnlyList<String> files = ModComponent.Instance.ModFiles.FindAllStartedWith(assetsRoot);
            if (files.Count == 0)
                return;

            Int32 languageIndex = Source.GetLanguageIndex(LocalizationManager.CurrentLanguage);
            Modify(languageIndex, files, Source);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void Modify(Int32 languageIndex, IReadOnlyList<String> files, LanguageSourceData sourceData)
    {
        Dictionary<String, TransifexEntry> entries = new Dictionary<String, TransifexEntry>();
        Dictionary<String, TransifexEntry> tags = new Dictionary<String, TransifexEntry>();

        foreach (IGrouping<String, String> filesInFolder in files.GroupBy(Path.GetDirectoryName))
        {
            String[] ordered = filesInFolder.OrderBy(File.GetLastWriteTimeUtc).ToArray();
            ReadJson(ordered, entries, tags);
        }

        ApplyTags(entries, tags);
        ApplyEntries(entries, languageIndex, sourceData);
        sourceData.UpdateDictionary(true);
        LocalizationManager.LocalizeAll(true);
    }

    private static void ReadJson(IReadOnlyList<String> files, Dictionary<String, TransifexEntry> entries, Dictionary<String, TransifexEntry> tags)
    {
        foreach (String filePath in files)
        {
            try
            {
                String extension = Path.GetExtension(filePath);
                String fileName = Path.GetFileNameWithoutExtension(filePath);

                switch (extension)
                {
                    case ".resjson":
                    {
                        ReadJson(filePath, fileName.Contains("Tags", StringComparison.InvariantCultureIgnoreCase) ? tags : entries);
                        break;
                    }
                    default:
                    {
                        ModComponent.Log.LogWarning($"File with the {extension} extension cannot be used as a localization source. File: {filePath}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ModComponent.Log.LogException(ex, $"Failed to apply file [{filePath}].");
            }
        }
    }

    private static void ReadJson(String filePath, Dictionary<String, TransifexEntry> entries)
    {
        OrderedDictionary<String, TransifexEntry> data = StructuredJson.Read(filePath);
        foreach ((String key, TransifexEntry value) in data)
        {
            String nativeKey = key.StartsWith("$") ? key.Substring(1) : key; // $msg_mena_bar1 -> msg_mena_bar1
            entries[nativeKey] = value;
        }
    }


    private static void ApplyTags(Dictionary<String, TransifexEntry> entries, Dictionary<String, TransifexEntry> tags)
    {
        if (tags.Count == 0)
            return;

        Reference<TextReplacement>[] replacements = tags
            .Select(t => new Reference<TextReplacement>('{' + t.Key + '}', t.Value.Text))
            .ToArray();

        Int32 changed = 0;
        foreach ((String key, TransifexEntry value) in entries)
        {
            String original = value.Text;
            value.Text = original.ReplaceAll(replacements);
            if (original != value.Text)
                changed++;
        }

        ModComponent.Log.LogMessage($"[{nameof(LocalizationManager_AddSource)}] Applied: Tags.resjson. Changed: {changed}");
    }

    private static void ApplyEntries(Dictionary<String, TransifexEntry> entries, Int32 languageIndex, LanguageSourceData sourceData)
    {
        Int32 changed = 0;
        Int32 skipped = 0;
        foreach ((String key, TransifexEntry value) in entries)
        {
            TermData termData = sourceData.GetTermData(key);
            if (termData is not null)
            {
                termData.SetTranslation(languageIndex, value.Text);
                changed++;
            }
            else
            {
                // info = new LocalizedMessages.MessageInfo { m_Value = value.Text };
                // messageInfos.Add(key, info);
                skipped++;
            }
        }

        ModComponent.Log.LogMessage($"[{nameof(LocalizationManager_AddSource)}] Applied: {entries.Count} localized files. Changed: {changed}, Skipped: {skipped}");
    }
}