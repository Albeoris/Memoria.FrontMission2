using System;
using BepInEx.Logging;

namespace Memoria.FrontMission2.Configuration;

public sealed partial class ModConfiguration
{
    public SpeedConfiguration Speed { get; }
    public SavesConfiguration Saves { get; }
    public AssetsConfiguration Assets { get; }
    public ArenaConfiguration Arena { get; }
    public BattlefieldConfiguration Battlefield { get; }
    public LocalizationConfiguration Localization { get; }
    public DebugConfiguration Debug { get; }

    private readonly ConfigFileProvider _provider = new();

    public ModConfiguration()
    {
        using (var log = Logger.CreateLogSource("Memoria Config"))
        {
            try
            {
                log.LogInfo($"Initializing {nameof(ModConfiguration)}");

                Speed = SpeedConfiguration.Create(_provider);
                Saves = SavesConfiguration.Create(_provider);
                Assets = AssetsConfiguration.Create(_provider);
                Arena = ArenaConfiguration.Create(_provider);
                Battlefield = BattlefieldConfiguration.Create(_provider);
                Localization = LocalizationConfiguration.Create(_provider);
                Debug = DebugConfiguration.Create(_provider);

                log.LogInfo($"{nameof(ModConfiguration)} initialized successfully.");
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to initialize {nameof(ModConfiguration)}: {ex}");
                throw;
            }
        }
    }

    public void OverrideFrom(String[] configDirectories)
    {
        // TODO: Reset previous overrides
        if (configDirectories.Length == 0)
            return;

        using (var log = Logger.CreateLogSource("Memoria Config"))
        {
            if (configDirectories.Length == 1)
                log.LogInfo($"Loading external config files from directory:");
            else
                log.LogInfo($"Loading external config files from {configDirectories.Length} directories:");

            foreach (String configDirectory in configDirectories)
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(configDirectory);
                log.LogInfo("    " + shortPath);

                try
                {
                    ConfigFileProvider provider = new(configDirectory, saveChangedConfigFiles: false);
                    _provider.SaveChangedConfigFiles = false;

                    Speed.OverrideFrom(SpeedConfiguration.Create(provider));
                    Saves.OverrideFrom(SavesConfiguration.Create(provider));
                    Assets.OverrideFrom(AssetsConfiguration.Create(provider));
                    Arena.OverrideFrom(ArenaConfiguration.Create(provider));
                    Battlefield.OverrideFrom(BattlefieldConfiguration.Create(provider));
                    Localization.OverrideFrom(LocalizationConfiguration.Create(provider));
                    Debug.OverrideFrom(DebugConfiguration.Create(provider));
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to load external config files from {shortPath}: {ex}");
                }
                finally
                {
                    _provider.SaveChangedConfigFiles = true;
                }
            }
        }
    }
}