using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria.FrontMission2.Shared.Framework.UIChanger;

public sealed class SharedStretchableGarageSelectionHighlight : IStretchableObject
{
    public RectTransform RectTransform { get; }
            
    private readonly Image _image;

    public SharedStretchableGarageSelectionHighlight(RectTransform rectTransform, Image image)
    {
        RectTransform = rectTransform;
        _image = image;
    }

    public void MakeStretchable()
    {
        _image.type = Image.Type.Sliced;
        RectTransform.SetPivot(PivotPreset.MiddleLeft, keepCurrentRect: true);
        RectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
    }

    public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out SharedStretchableGarageSelectionHighlight result, out FormattableString reason)
    {
        result = null;

        const String expectedName = "selected";
        if (!objects.TryGetValue(expectedName, out RectTransform rect))
        {
            reason = $"Cannot find object by name [{expectedName}].";
            return false;
        }

        Image image = rect.GetComponent<Image>();
        if (image is null)
        {
            reason = $"The [{rect.name}] does not have an Image component.";
            return false;
        }

        if (image.type != Image.Type.Simple && image.type != Image.Type.Sliced)
        {
            reason = $"The Image type of [{rect.name}] must be either [{nameof(Image.Type.Simple)}] or [{nameof(Image.Type.Sliced)}], but the current type is [{image.type}].";
            return false;
        }

        Sprite sprite = image.sprite;
        if (sprite is null)
        {
            reason = $"The Image component of [{rect.name}] does not have a sprite assigned.";
            return false;
        }

        const String expectedSpriteName = "Wanzer_Setup_Selected2_with_scroll_bar";
        if (sprite.name != expectedSpriteName)
        {
            reason = $"The sprite [{sprite.name}] of [{rect.name}] must be named [{expectedSpriteName}].";
            return false;
        }

        Vector4 expectedBorders = new Vector4(46, 0, 0, 0);
        if (sprite.border != expectedBorders)
        {
            reason = $"The border of sprite [{sprite.name}] of [{rect.name}] must be {expectedBorders}, but the current border is {sprite.border}.";
            return false;
        }

        result = new SharedStretchableGarageSelectionHighlight(rect, image);
        reason = null;
        return true;
    }
}