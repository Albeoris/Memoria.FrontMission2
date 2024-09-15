using System;
using System.Linq;
using Memoria.FrontMission2.Shared.Framework.UIChanger;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria.FrontMission2.HarmonyHooks;

public class DebugHighlightObject : MonoBehaviour
{
    private static Color[] HighlightColors =
    {
        new Color(1f, 0f, 0f, 0.5f),
        new Color(0f, 1f, 0f, 0.5f),
        new Color(0f, 0f, 1f, 0.5f),
        new Color(1f, 1f, 0f, 0.5f),
        new Color(0f, 1f, 1f, 0.5f),
        new Color(1f, 0f, 1f, 0.5f),

        new Color(1f, 0.5f, 0f, 0.5f),
        new Color(0f, 1f, 0.5f, 0.5f),
        new Color(0.5f, 0f, 1f, 0.5f),
        new Color(1f, 1f, 0.5f, 0.5f),
        new Color(0.5f, 1f, 1f, 0.5f),
        new Color(1f, 0.5f, 1f, 0.5f),
    };

    private static Int32 CurrentColorIndex = 0;
    
    private RectTransform _rectTransform;
    private GameObject _highlightObject;
    private Image _highlightImage;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        
        _highlightObject = new GameObject("DBG_Highlight");
        _highlightObject.transform.SetParent(transform, false);

        _highlightImage = _highlightObject.AddComponent<Image>();
        _highlightImage.color = HighlightColors[(CurrentColorIndex++) % HighlightColors.Length];

        RectTransform highlightRectTransform = _highlightObject.GetComponent<RectTransform>();
        highlightRectTransform.anchorMin = Vector2.zero;
        highlightRectTransform.anchorMax = Vector2.one;
        highlightRectTransform.offsetMin = Vector2.zero;
        highlightRectTransform.offsetMax = Vector2.zero;

        // Перемещаем highlight под Image
        _highlightObject.transform.SetSiblingIndex(0); // Перемещает в начало, за основной Image
    }

    // Метод для управления включением/выключением выделения
    public void SetHighlight(bool enabled)
    {
        if (_highlightObject != null)
        {
            _highlightObject.SetActive(enabled);
        }
    }

    public void SetAnchors(AnchorPreset preset)
    {
        _rectTransform.SetAnchors(preset, keepCurrentRect: true);
    }
    
    public void SetPivot(PivotPreset preset)
    {
        _rectTransform.SetPivot(preset, keepCurrentRect: true);
    }
}