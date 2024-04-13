using System;
using Memoria.FrontMission2.Configuration.Hotkey;
using Memoria.FrontMission2.HarmonyHooks;
using UnityEngine;

namespace Memoria.FrontMission2.Core;

public sealed class LocalizationControl : SafeComponent
{
    public LocalizationControl()
    {
    }

    private Hotkey RepeatPrevious { get; } = new(KeyCode.F2) { Shift = true };
    private Hotkey RepeatCurrent { get; } = new(KeyCode.F3) { Shift = true };
    private Hotkey OpenInBrowser { get; } = new(KeyCode.F4) { Shift = true };

    protected override void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        // if (InputManager.IsToggled(RepeatPrevious))
        //     TalkGroup_GetNextCommand.RepeatPrevious = true;
        //
        // if (InputManager.IsToggled(RepeatCurrent))
        //     TalkGroup_GetNextCommand.RepeatLast = true;

        if (InputManager.IsToggled(OpenInBrowser))
        {
            // String messageId = TalkGroup_GetNextCommand.LastMessageId;
            // if (messageId != null)
            //     System.Diagnostics.Process.Start($"https://app.transifex.com/Albeoris/search/?q=key%3A{messageId}");
        }
    }
}