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
        Single desiredHeight = ModComponent.Instance.Config.UI.GaragePaintWanzerPanelHeight;
        if (desiredHeight < 1)
            return;
        
        GameObject panel = garageCanvas.FindChildByName("PaintWanzerPanel");
        if (panel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [PaintWanzerPanel] inside [{garageCanvas.name}].");
            return;
        }

        if (!StretchableGarageSmallScroll.TryMakeStretchable(panel, out IStretchableObject stretchableObject, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{panel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableObject.RectTransform.sizeDelta;
        desiredHeight = desiredHeight > 0 ? desiredHeight : oldSize.y;

        Vector2 newSize = oldSize with { y = desiredHeight };
        stretchableObject.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableObject.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
    }

    private void StretchChooseWanzerPanel(GameObject garageCanvas)
    {
        Single desiredHeight = ModComponent.Instance.Config.UI.GarageChooseWanzerPanelHeight;
        if (desiredHeight < 1)
            return;
        
        GameObject panel = garageCanvas.FindChildByName("ChooseWanzerPanel");
        if (panel is null)
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot find [ChooseWanzerPanel] inside [{garageCanvas.name}].");
            return;
        }

        if (!StretchableGarageSmallScroll.TryMakeStretchable(panel, out IStretchableObject stretchableObject, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{panel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableObject.RectTransform.sizeDelta;
        desiredHeight = desiredHeight > 0 ? desiredHeight : oldSize.y;

        Vector2 newSize = oldSize with { y = desiredHeight };
        stretchableObject.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableObject.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
    }

    private void StretchEquipWeaponPanel(GameObject garageCanvas)
    {
        Single desiredWith = ModComponent.Instance.Config.UI.GarageEquipWeaponPanelWidth;
        Single desiredHeight = ModComponent.Instance.Config.UI.GarageEquipWeaponPanelHeight;
        if (desiredWith < 1 && desiredHeight < 1)
            return;
        
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

        if (!StretchableGarageSmallScroll.TryMakeStretchable(equipWeaponPanel, out IStretchableObject stretchableObject, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{equipWeaponPanel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableObject.RectTransform.sizeDelta;
        desiredWith = desiredWith > 0 ? desiredWith : oldSize.x;
        desiredHeight = desiredHeight > 0 ? desiredHeight : oldSize.y;

        Vector2 newSize = new Vector2(x: desiredWith, y: desiredHeight);
        stretchableObject.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableObject.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
        
        // Move WeaponInfoPanel to right
        Single oxDelta = desiredWith - 150;
        Transform weaponInfoTransform = weaponInfoPanel.transform;
        weaponInfoTransform.localPosition = new Vector2(-350 + oxDelta, 340);
        
        RectTransform weaponStartsTransform = weaponInfoPanel.FindChildByName("WeaponStats")?.transform as RectTransform;
        if (weaponStartsTransform is null)
        {
            ModComponent.Log.LogError($"Cannot find WeaponStats to change its width.");
        }
        else
        {
            weaponStartsTransform.sizeDelta = weaponStartsTransform.sizeDelta with { x = 50 };
            weaponInfoTransform.localPosition = new Vector2(-325 + oxDelta, 340);
        }
    }
    
    private void StretchEquipPartPanel(GameObject garageCanvas)
    {
        Single desiredWith = ModComponent.Instance.Config.UI.GarageEquipPartPanelWidth;
        Single desiredHeight = ModComponent.Instance.Config.UI.GarageEquipPartPanelHeight;
        if (desiredWith < 1 && desiredHeight < 1)
            return;
        
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
        
        if (!StretchableGarageSmallScroll.TryMakeStretchable(equipPartPanel, out IStretchableObject stretchableEquipPanel, out FormattableString reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{equipPartPanel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }
        
        if (!StretchableGaragePartInfoPanel.TryMakeStretchable(partInfoPanel, out IStretchableObject stretchableInfoPanel, out reason))
        {
            ModComponent.Log.LogError($"[{nameof(ModComponent)}].{nameof(OnSceneLoaded)}(): Cannot stretch [{partInfoPanel.name}] inside [{garageCanvas.name}]. Reason: {reason}");
            return;
        }

        Vector2 oldSize = stretchableEquipPanel.RectTransform.sizeDelta;
        desiredWith = desiredWith > 0 ? desiredWith : oldSize.x;
        desiredHeight = desiredHeight > 0 ? desiredHeight : oldSize.y;

        Vector2 newSize = new Vector2(x: desiredWith, y: desiredHeight);
        stretchableEquipPanel.RectTransform.sizeDelta = newSize;
        ModComponent.Log.LogInfo($"[{stretchableEquipPanel.RectTransform.name}] has been stretched {oldSize} -> {newSize}.");
        
        // Move PartInfoPanel to right
        Single oxDelta = desiredWith - 150;
        RectTransform partInfoTransform = stretchableInfoPanel.RectTransform;
        partInfoTransform.localPosition = new Vector2(-510 + oxDelta, 487);
        partInfoTransform.sizeDelta = partInfoTransform.sizeDelta with { x = 460 };
    }
}