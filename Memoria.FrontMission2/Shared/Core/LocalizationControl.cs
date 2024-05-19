using System;
using Memoria.FrontMission2.Configuration.Hotkey;
using Memoria.FrontMission2.HarmonyHooks;
using UnityEngine;
using UnityEngine.Networking;

namespace Memoria.FrontMission2.Core;

public sealed class LocalizationControl : SafeComponent
{
    public LocalizationControl()
    {
    }

    private Hotkey OpenInBrowserShift { get; } = new(KeyCode.F3) { Shift = true };
    private Hotkey OpenInBrowserControl { get; } = new(KeyCode.F3) { Control = true };

    protected override void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (InputManager.IsToggled(OpenInBrowserShift) || InputManager.IsToggled(OpenInBrowserControl))
        {
            if (!String.IsNullOrEmpty(DialogueManager_PlayNextSentence.LastMessageText))
            {
                
            }
        }
    }
}