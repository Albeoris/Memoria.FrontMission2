using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.FrontMission2.Shared.Framework.Unity;

internal static class ExtensionMethodsTransform
{
    public static IEnumerable<Transform> EnumerateChildren(this Transform parent)
    {
        if (parent is null) throw new ArgumentNullException(nameof(parent));

        for (Int32 i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            yield return child;
        }
    }
}