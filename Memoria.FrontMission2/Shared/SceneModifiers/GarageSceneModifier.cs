using System;
using System.Linq;
using HarmonyLib;
using Memoria.FrontMission2.HarmonyHooks.ColorfulStatIconsInGarage;
using Memoria.FrontMission2.Shared.Framework.UIChanger;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Memoria.FrontMission2;

public sealed class GarageSceneModifier : ISceneModifier
{
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();
        GameObject garageCanvas = rootObjects.FirstOrDefault(o => o.name == "GarageCanvas");
        if (garageCanvas is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [GarageCanvas].");
            return;
        }

        StretchPaintWanzerPanel(garageCanvas);
        StretchChooseWanzerPanel(garageCanvas);
        StretchEquipWeaponPanel(garageCanvas);
        StretchEquipPartPanel(garageCanvas);
    }

    private void StretchPaintWanzerPanel(GameObject garageCanvas)
    {
        GameObject panel = garageCanvas.FindChildByName("PaintWanzerPanel");
        if (panel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [PaintWanzerPanel] inside [{garageCanvas.name}].");
            return;
        }

        if (!StretchableObjectMaker.TryMakeStretchable(panel, out IStretchableObject stretchableObject, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{panel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableObject.RectTransform.sizeDelta;
        Vector2 newSize = oldSize with { y = 540 };
        stretchableObject.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableObject.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
    }

    private void StretchChooseWanzerPanel(GameObject garageCanvas)
    {
        GameObject panel = garageCanvas.FindChildByName("ChooseWanzerPanel");
        if (panel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [ChooseWanzerPanel] inside [{garageCanvas.name}].");
            return;
        }

        if (!StretchableObjectMaker.TryMakeStretchable(panel, out IStretchableObject stretchableObject, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{panel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableObject.RectTransform.sizeDelta;
        Vector2 newSize = oldSize with { y = 300 };
        stretchableObject.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableObject.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
    }

    private void StretchEquipWeaponPanel(GameObject garageCanvas)
    {
        GameObject equipWeaponPanel = garageCanvas.FindChildByName("EquipWeaponPanel");
        if (equipWeaponPanel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [EquipWeaponPanel] inside [{garageCanvas.name}].");
            return;
        }

        GameObject weaponInfoPanel = garageCanvas.FindChildByName("WeaponInfoPanel");
        if (weaponInfoPanel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [WeaponInfoPanel] inside [{garageCanvas.name}].");
            return;
        }

        if (!StretchableObjectMaker.TryMakeStretchable(equipWeaponPanel, out IStretchableObject stretchableObject, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{equipWeaponPanel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableObject.RectTransform.sizeDelta;
        Vector2 newSize = oldSize with { y = 600 };
        stretchableObject.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableObject.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
        
        // Move WeaponInfoPanel to right
        Transform weaponInfoTransform = weaponInfoPanel.transform;
        weaponInfoTransform.localPosition = new Vector2(-400, 340);
    }
    
    private void StretchEquipPartPanel(GameObject garageCanvas)
    {
        GameObject equipPartPanel = garageCanvas.FindChildByName("EquipPartPanel");
        if (equipPartPanel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [EquipPartPanel] inside [{garageCanvas.name}].");
            return;
        }

        GameObject partInfoPanel = garageCanvas.FindChildByName("PartInfoPanel");
        if (partInfoPanel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [PartInfoPanel] inside [{garageCanvas.name}].");
            return;
        }

        if (!StretchableObjectMaker.TryMakeStretchable(equipPartPanel, out IStretchableObject stretchableObject, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{equipPartPanel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableObject.RectTransform.sizeDelta;
        Vector2 newSize = oldSize with { y = 600 };
        stretchableObject.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableObject.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
        
        // Move PartInfoPanel to right
        Transform partInfoTransform = partInfoPanel.transform;
        partInfoTransform.localPosition = new Vector2(-270, 270);
    }
}