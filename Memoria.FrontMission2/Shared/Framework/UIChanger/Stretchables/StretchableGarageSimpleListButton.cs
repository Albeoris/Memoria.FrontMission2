using System;
using System.Collections.Generic;
using System.Linq;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria.FrontMission2.Shared.Framework.UIChanger;

public static class StretchableGarageSimpleListButton
{
    public static Boolean TryMakeStretchable(RemoveWeapon obj, out IStretchableObject stretchableObject, out FormattableString reason)
    {
        return TryMakeStretchableInternal(obj, out stretchableObject, out reason);
    }
    
    public static Boolean TryMakeStretchable(CurrentWeaponInfo obj, out IStretchableObject stretchableObject, out FormattableString reason)
    {
        return TryMakeStretchableInternal(obj, out stretchableObject, out reason);
    }
    
    public static Boolean TryMakeStretchable(CurrentPartInfo obj, out IStretchableObject stretchableObject, out FormattableString reason)
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

        if (!Button.TryCreate(rectTransform, out Button stretchable, out reason))
        {
            reason = $"Failed to create {nameof(Button)} for the specified RectTransform. Reason: {reason}";
            return false;
        }

        stretchable.MakeStretchable();
        stretchableObject = stretchable;
        reason = null;
        return true;
    }

    private sealed class Button : IStretchableObject
    {
        public RectTransform RectTransform { get; }

        private readonly SharedStretchableGarageSelectionHighlight _selectionHighlight;
        private readonly SharedStretchableGarageButtonText _buttonText;

        public Button(RectTransform rectTransform, SharedStretchableGarageSelectionHighlight selectionHighlight, SharedStretchableGarageButtonText buttonText)
        {
            RectTransform = rectTransform;

            _selectionHighlight = selectionHighlight;
            _buttonText = buttonText;
        }

        public void MakeStretchable()
        {
            _selectionHighlight.MakeStretchable();
            _buttonText.MakeStretchable();
        }

        public static Boolean TryCreate(RectTransform rectTransform, out Button result, out FormattableString reason)
        {
            result = null;

            Int32 childCount = rectTransform.childCount;
            const Int32 expectedCount = 2;
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

            if (!SharedStretchableGarageButtonText.TryCreate(childrenByName, out SharedStretchableGarageButtonText weaponName, out reason))
            {
                reason = $"Failed to create {nameof(SharedStretchableGarageButtonText)} for the specified RectTransform. Reason: {reason}";
                return false;
            }

            result = new Button(rectTransform, selectionHighlight, weaponName);
            return true;
        }
    }
}