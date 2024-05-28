using System;

namespace Memoria.FrontMission2.Configuration;

[ConfigScope("Localization")]
public abstract partial class LocalizationConfiguration
{
    [ConfigEntry("When enabled, prohibits changing character names during the game." +
                 "$[Russian]: При включении, запрещает менять имена персонажей во время игры.")]
    public virtual Boolean ProhibitsChangingCharacterNames => false;
    
    [ConfigEntry("When enabled, prohibits changing character call signs during the game." +
                 "$[Russian]: При включении, запрещает менять позывные персонажей во время игры.")]
    public virtual Boolean ProhibitsChangingCharacterCallSigns => false;
    
    [ConfigEntry("Use character names instead of call signs." +
                 "$[Russian]: Использовать имена героев вместо позывных.")]
    public virtual CallSignsFromNames UseCharacterNamesInsteadCallSigns => CallSignsFromNames.Disabled;

    public abstract void CopyFrom(LocalizationConfiguration configuration);
    public abstract void OverrideFrom(LocalizationConfiguration configuration);
    
    public enum CallSignsFromNames
    {
        Disabled = 0,
        NormalCase = 1,
        UpperCase = 2
    }
}