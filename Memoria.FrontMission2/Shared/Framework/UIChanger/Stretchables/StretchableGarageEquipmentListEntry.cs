using System;
using System.Collections.Generic;
using System.Linq;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;

namespace Memoria.FrontMission2.Shared.Framework.UIChanger;

public static class StretchableGarageEquipmentListEntry
{
    public static Boolean TryMakeStretchable(WeaponButtonGarage obj, out IStretchableObject stretchableObject, out FormattableString reason)
    {
        return TryMakeStretchableInternal(obj, out stretchableObject, out reason);
    }
    
    public static Boolean TryMakeStretchable(PartButtonGarage obj, out IStretchableObject stretchableObject, out FormattableString reason)
    {
        return TryMakeStretchableInternal(obj, out stretchableObject, out reason);
    }
    
    private static Boolean TryMakeStretchableInternal(MonoBehaviour obj, out IStretchableObject stretchableObject, out FormattableString reason)
    {
        stretchableObject = null;

        if (obj.transform is not RectTransform rectTransform)
        {
            reason = $"The GameObject [{obj.name}] does not have a RectTransform component.";
            return false;
        }

        if (!EquipmentButton.TryCreate(rectTransform, out EquipmentButton stretchable, out reason))
        {
            reason = $"Failed to create {nameof(EquipmentButton)} for the specified RectTransform. Reason: {reason}";
            return false;
        }

        stretchable.MakeStretchable();
        stretchableObject = stretchable;
        reason = null;
        return true;
    }

    private sealed class EquipmentButton : IStretchableObject
    {
        public RectTransform RectTransform { get; }

        private readonly SharedStretchableGarageSelectionHighlight _garageSelectionHighlight;
        private readonly EquipmentName _equipmentName;
        private readonly EquipmentCount _equipmentCount;

        public EquipmentButton(RectTransform rectTransform, SharedStretchableGarageSelectionHighlight garageSelectionHighlight, EquipmentName equipmentName, EquipmentCount equipmentCount)
        {
            RectTransform = rectTransform;

            _garageSelectionHighlight = garageSelectionHighlight;
            _equipmentName = equipmentName;
            _equipmentCount = equipmentCount;
        }

        public void MakeStretchable()
        {
            _garageSelectionHighlight.MakeStretchable();
            _equipmentName.MakeStretchable();
            _equipmentCount.MakeStretchable();
        }

        public static Boolean TryCreate(RectTransform rectTransform, out EquipmentButton result, out FormattableString reason)
        {
            result = null;

            Int32 childCount = rectTransform.childCount;
            const Int32 expectedCount = 3;
            if (childCount < expectedCount)
            {
                reason = $"The RectTransform must have at least {expectedCount} children to be stretchable, but the GameObject [{rectTransform.name}] has {childCount}.";
                return false;
            }

            IReadOnlyDictionary<String, RectTransform> childrenByName = rectTransform.GetChildrenByName();

            if (!SharedStretchableGarageSelectionHighlight.TryCreate(childrenByName, out SharedStretchableGarageSelectionHighlight selectionHighlight, out reason))
            {
                reason = $"Failed to create {nameof(SharedStretchableGarageSelectionHighlight)} for the specified RectTransform. Reason: {reason}";
                return false;
            }

            if (!EquipmentName.TryCreate(childrenByName, out EquipmentName weaponName, out reason))
            {
                reason = $"Failed to create {nameof(EquipmentName)} for the specified RectTransform. Reason: {reason}";
                return false;
            }

            if (!EquipmentCount.TryCreate(childrenByName, out EquipmentCount weaponCount, out reason))
            {
                reason = $"Failed to create {nameof(EquipmentCount)} for the specified RectTransform. Reason: {reason}";
                return false;
            }

            result = new EquipmentButton(rectTransform, selectionHighlight, weaponName, weaponCount);
            return true;
        }

        public sealed class EquipmentName
        {
            private readonly RectTransform _rectTransform;

            public EquipmentName(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }

            public void MakeStretchable()
            {
                _rectTransform.SetPivot(PivotPreset.MiddleLeft, keepCurrentRect: true);
                _rectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
            }

            public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out EquipmentName result, out FormattableString reason)
            {
                result = null;

                const String expectedName = "MaskName";
                if (!objects.TryGetValue(expectedName, out RectTransform rectTransform))
                {
                    reason = $"Cannot find object by name [{expectedName}].";
                    return false;
                }

                result = new EquipmentName(rectTransform);
                reason = null;
                return true;
            }
        }

        public sealed class EquipmentCount
        {
            private readonly RectTransform _rectTransform;

            public EquipmentCount(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }

            public void MakeStretchable()
            {
                _rectTransform.SetPivot(PivotPreset.MiddleRight, keepCurrentRect: true);
                _rectTransform.SetAnchors(AnchorPreset.MiddleRight, keepCurrentRect: true);
            }

            public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out EquipmentCount result, out FormattableString reason)
            {
                result = null;

                const String expectedName = "Count";
                if (!objects.TryGetValue(expectedName, out RectTransform rect))
                {
                    reason = $"Cannot find object by name [{expectedName}].";
                    return false;
                }

                result = new EquipmentCount(rect);
                reason = null;
                return true;
            }
        }
    }
}