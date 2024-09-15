using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.FrontMission2.Shared.Framework.UIChanger;

public sealed class SharedStretchableGarageButtonText : IStretchableObject
{
    public RectTransform RectTransform { get; }

    public SharedStretchableGarageButtonText(RectTransform rectTransform)
    {
        RectTransform = rectTransform;
    }

    public void MakeStretchable()
    {
        RectTransform.SetPivot(PivotPreset.MiddleLeft, keepCurrentRect: true);
        RectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
    }

    public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out SharedStretchableGarageButtonText result, out FormattableString reason)
    {
        result = null;

        const String expectedName = "text";
        if (!objects.TryGetValue(expectedName, out RectTransform rectTransform))
        {
            reason = $"Cannot find object by name [{expectedName}].";
            return false;
        }

        result = new SharedStretchableGarageButtonText(rectTransform);
        reason = null;
        return true;
    }
}