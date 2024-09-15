using System;
using System.Collections.Generic;
using System.Linq;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Memoria.FrontMission2.Shared.Framework.UIChanger;

public static class StretchableGarageSmallScroll
{
    public static Boolean TryMakeStretchable(GameObject obj, out IStretchableObject stretchableObject, out FormattableString reason)
    {
        stretchableObject = null;
		
        if (obj.transform is not RectTransform rectTransform)
        {
            reason = $"The GameObject [{obj.name}] does not have a RectTransform component.";
            return false;
        }
		
        if (!SmallScroll.TryCreate(rectTransform, out SmallScroll smallScroll, out reason))
        {
            reason = $"Failed to create {nameof(SmallScroll)} for the specified RectTransform. Reason: {reason}";
            return false;
        }

        smallScroll.MakeStretchable();
        stretchableObject = smallScroll;
        reason = null;
        return true;
    }
	
    private sealed class SmallScroll : IStretchableObject
    {
        public RectTransform RectTransform { get; }
		
        private readonly FrameBackground _frameBackground;
        private readonly BlackBackground _blackBackground;
        private readonly Header _header;
        private readonly ScrollView _scrollView;

        public SmallScroll(RectTransform rectTransform, FrameBackground frameBackground, BlackBackground blackBackground, Header header, ScrollView scrollView)
        {
            RectTransform = rectTransform;
			
            _frameBackground = frameBackground;
            _blackBackground = blackBackground;
            _header = header;
            _scrollView = scrollView;
        }

        public void MakeStretchable()
        {
            _frameBackground.MakeStretchable();
            _blackBackground.MakeStretchable();
            _header.MakeStretchable();
            _scrollView.MakeStretchable();
        }

        public static Boolean TryCreate(RectTransform rectTransform, out SmallScroll result, out FormattableString reason)
        {
            result = null;
            
            Int32 childCount = rectTransform.childCount;
            const Int32 expectedCount = 4;
            if (childCount < expectedCount)
            {
                reason = $"The RectTransform must have at least {expectedCount} children to be stretchable, but the GameObject [{rectTransform.name}] has {childCount}.";
                return false;
            }

            IReadOnlyDictionary<String, RectTransform> childrenByName = rectTransform.GetChildrenByName();

            if (!FrameBackground.TryCreate(childrenByName, out FrameBackground frameBackground, out reason))
            {
                reason = $"Failed to create FrameBackground for the specified RectTransform. Reason: {reason}";
                return false;
            }
            
            if (!Header.TryCreate(childrenByName, out Header header, out reason))
            {
                reason = $"Failed to create Header for the specified RectTransform. Reason: {reason}";
                return false;
            }
			
            if (!BlackBackground.TryCreate(childrenByName, out BlackBackground blackBackground, out reason))
            {
                reason = $"Failed to create BlackBackground for the specified RectTransform. Reason: {reason}";
                return false;
            }
			
            if (!ScrollView.TryCreate(childrenByName, out ScrollView scrollView, out reason))
            {
                reason = $"Failed to create ScrollView for the specified RectTransform. Reason: {reason}";
                return false;
            }

            result = new SmallScroll(rectTransform, frameBackground, blackBackground, header, scrollView);
            return true;
        }

        public sealed class FrameBackground
        {
            private readonly RectTransform _rectTransform;
            private readonly Image _image;

            public FrameBackground(RectTransform rectTransform, Image image)
            {
                _rectTransform = rectTransform;
                _image = image;
            }

            public void MakeStretchable()
            {
                _image.type = Image.Type.Sliced;
                _rectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
            }

            public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out FrameBackground result, out FormattableString reason)
            {
                result = null;

                if (!objects.TryGetValue("FrameBackground", out RectTransform frameBackgroundTransform))
                {
                    reason = $"Cannot find object by name [FrameBackground].";
                    return false;
                }

                GameObject frameBackground = frameBackgroundTransform.gameObject;
                Image frameBackgroundImage = frameBackground.GetComponent<Image>();
                if (frameBackgroundImage is null)
                {
                    reason = $"The [{frameBackground.name}] does not have an Image component.";
                    return false;
                }

                if (frameBackgroundImage.type != Image.Type.Simple && frameBackgroundImage.type != Image.Type.Sliced)
                {
                    reason = $"The Image type of [{frameBackground.name}] must be either [Simple] or [Sliced], but the current type is [{frameBackgroundImage.type}].";
                    return false;
                }

                Sprite frameBackgroundSprite = frameBackgroundImage.sprite;
                if (frameBackgroundSprite is null)
                {
                    reason = $"The Image component of [{frameBackground.name}] does not have a sprite assigned.";
                    return false;
                }

                if (frameBackgroundSprite.name != "Scroll_Frame_Small")
                {
                    reason = $"The sprite [{frameBackgroundSprite.name}] of [{frameBackground.name}] must be named [Scroll_Frame_Small].";
                    return false;
                }

                Vector4 frameBackgroundSpriteExpectedBorders = new Vector4(12, 13, 42, 56);
                if (frameBackgroundSprite.border != frameBackgroundSpriteExpectedBorders)
                {
                    reason = $"The border of sprite [{frameBackgroundSprite.name}] of [{frameBackground.name}] must be {frameBackgroundSpriteExpectedBorders}, but the current border is {frameBackgroundSprite.border}.";
                    return false;
                }

                result = new FrameBackground(frameBackgroundTransform, frameBackgroundImage);
                reason = null;
                return true;
            }
        }
		
        public sealed class BlackBackground
        {
            private readonly RectTransform _rectTransform;

            public BlackBackground(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }

            public void MakeStretchable()
            {
                _rectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
            }

            public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out BlackBackground result, out FormattableString reason)
            {
                result = null;

                if (!objects.TryGetValue("BlackBackground", out RectTransform blackBackgroundTransformRectTransform))
                {
                    reason = $"Cannot find object by name [BlackBackground].";
                    return false;
                }

                GameObject blackBackground = blackBackgroundTransformRectTransform.gameObject;
                Image blackBackgroundImage = blackBackground.GetComponent<Image>();
                if (blackBackgroundImage is null)
                {
                    reason = $"The [{blackBackground.name}] does not have an Image component.";
                    return false;
                }

                if (blackBackgroundImage.sprite != null)
                {
                    reason = $"The [{blackBackground.name}] Image component has a sprite assigned.";
                    return false;
                }

                result = new BlackBackground(blackBackgroundTransformRectTransform);
                reason = null;
                return true;
            }
        }
        
        public sealed class Header
        {
            private readonly RectTransform _rectTransform;

            public Header(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }

            public void MakeStretchable()
            {
                _rectTransform.SetAnchors(AnchorPreset.TopCenter, keepCurrentRect: true);
            }

            public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out Header result, out FormattableString reason)
            {
                result = null;

                if (!objects.TryGetValue("Header", out RectTransform headerTransform))
                {
                    reason = $"Cannot find object by name [Header].";
                    return false;
                }

                result = new Header(headerTransform);
                reason = null;
                return true;
            }
        }
		
        public sealed class ScrollView
        {
            private readonly RectTransform _rectTransform;
            private readonly ScrollbarVertical _scrollbarVertical;

            public ScrollView(RectTransform rectTransform, ScrollbarVertical scrollbarVertical)
            {
                _rectTransform = rectTransform;
                _scrollbarVertical = scrollbarVertical;
            }

            public void MakeStretchable()
            {
                _rectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
                _scrollbarVertical.MakeStretchable();
            }
            
            public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, out ScrollView result, out FormattableString reason)
            {
                result = null;

                if (!objects.TryGetValue("Scroll View", out RectTransform scrollViewRectTransform))
                {
                    reason = $"Cannot find object by name [Scroll View].";
                    return false;
                }

                GameObject scrollView = scrollViewRectTransform.gameObject;
                Int32 childCount = scrollViewRectTransform.childCount;
                if (childCount != 2)
                {
                    reason = $"The RectTransform must have exactly 2 children to be stretchable, but the GameObject [{scrollViewRectTransform.name}] has {childCount}.";
                    return false;
                }

                ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
                if (scrollRect is null)
                {
                    reason = $"The [{scrollView.name}] does not have an {nameof(ScrollRect)} component.";
                    return false;
                }
                
                if (!ScrollbarVertical.TryCreate(scrollViewRectTransform.GetChild(1), out ScrollbarVertical scrollbarVertical, out reason))
                {
                    reason = $"Failed to create ScrollbarVertical for the specified RectTransform. Reason: {reason}";
                    return false;
                }

                result = new ScrollView(scrollViewRectTransform, scrollbarVertical);
                reason = null;
                return true;
            }
        }
        
        public sealed class ScrollbarVertical
        {
            private readonly RectTransform _arrowUpRectTransform;
            private readonly RectTransform _arrowDownRectTransform;

            public ScrollbarVertical(RectTransform arrowUpRectTransform, RectTransform arrowDownRectTransform)
            {
                _arrowUpRectTransform = arrowUpRectTransform;
                _arrowDownRectTransform = arrowDownRectTransform;
            }

            public void MakeStretchable()
            {
                _arrowUpRectTransform.SetAnchors(AnchorPreset.TopCenter, keepCurrentRect: true);
                _arrowDownRectTransform.SetAnchors(AnchorPreset.BottomCenter, keepCurrentRect: true);
            }

            public static Boolean TryCreate(Transform transform, out ScrollbarVertical result, out FormattableString reason)
            {
                result = null;

                if (transform is not RectTransform rectTransform)
                {
                    reason = $"The GameObject [{transform.name}] does not have a RectTransform component.";
                    return false;
                }

                GameObject scrollbarVertical = rectTransform.gameObject;
                if (scrollbarVertical.name != "Scrollbar Vertical")
                {
                    reason = $"The GameObject [{scrollbarVertical.name}] must be named [Scrollbar Vertical].";
                    return false;
                }
                
                Int32 childCount = rectTransform.childCount;
                if (childCount != 3)
                {
                    reason = $"The RectTransform must have exactly 3 children to be stretchable, but the GameObject [{rectTransform.name}] has {childCount}.";
                    return false;
                }

                Transform arrowUpTransform = rectTransform.GetChild(1);
                if (arrowUpTransform is not RectTransform arrowUpRectTransform)
                {
                    reason = $"The GameObject [{arrowUpTransform.name}] does not have a RectTransform component.";
                    return false;
                }
                
                GameObject arrowUp = arrowUpRectTransform.gameObject;
                if (arrowUp.name != "ArrowUp")
                {
                    reason = $"The GameObject [{arrowUp.name}] must be named [ArrowUp].";
                    return false;
                }
                
                Transform arrowDownTransform = rectTransform.GetChild(2);
                if (arrowDownTransform is not RectTransform arrowDownRectTransform)
                {
                    reason = $"The GameObject [{arrowDownTransform.name}] does not have a RectTransform component.";
                    return false;
                }
                
                GameObject arrowDown = arrowDownRectTransform.gameObject;
                if (arrowDown.name != "ArrowDown")
                {
                    reason = $"The GameObject [{arrowDown.name}] must be named [ArrowDown].";
                    return false;
                }
                

                result = new ScrollbarVertical(arrowUpRectTransform, arrowDownRectTransform);
                reason = null;
                return true;
            }
        }
    }
}