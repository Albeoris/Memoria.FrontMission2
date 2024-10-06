using System;
using System.Collections.Generic;
using System.Linq;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;

namespace Memoria.FrontMission2.Shared.Framework.UIChanger;

public static class StretchableGaragePartInfoPanel
{
    public static Boolean TryMakeStretchable(GameObject obj, out IStretchableObject stretchableObject, out FormattableString reason)
    {
        stretchableObject = null;
		
        if (obj.transform is not RectTransform rectTransform)
        {
            reason = $"The GameObject [{obj.name}] does not have a RectTransform component.";
            return false;
        }
		
        if (!Root.TryCreate(rectTransform, out Root root, out reason))
        {
            reason = $"Failed to create {nameof(Root)} for the specified RectTransform. Reason: {reason}";
            return false;
        }

        root.MakeStretchable();
        stretchableObject = root;
        reason = null;
        return true;
    }
	
    private sealed class Root : IStretchableObject
    {
        private readonly List<StatsContainer> _statsContainer;
        public RectTransform RectTransform { get; }
		
        private Root(RectTransform rectTransform, List<StatsContainer> statsContainer)
        {
            RectTransform = rectTransform;
            _statsContainer = statsContainer;
        }

        public void MakeStretchable()
        {
            foreach (StatsContainer container in _statsContainer)
                container.MakeStretchable();

            RectTransform.SetPivot(PivotPreset.TopLeft, keepCurrentRect: true);
            foreach (StatsContainer container in _statsContainer)
                AnchorPresets.ResizeAndMoveParentToChild(container.RectTransform);   
        }

        public static Boolean TryCreate(RectTransform rectTransform, out Root result, out FormattableString reason)
        {
            result = null;
            
            Int32 childCount = rectTransform.childCount;
            const Int32 expectedCount = 3;
            if (childCount < expectedCount)
            {
                reason = $"The RectTransform must have at least {expectedCount} children to be stretchable, but the GameObject [{rectTransform.name}] has {childCount}.";
                return false;
            }

            List<StatsContainer> containers = new();
            foreach (RectTransform child in rectTransform.EnumerateChildren())
            {
                if (!child.name.EndsWith("Stats"))
                    continue;

                if (!StatsContainer.TryCreate(child, out StatsContainer stats, out reason))
                {
                    reason = $"Failed to create {nameof(StatsContainer)} from [{child.name}]. Reason: {reason}";
                    return false;
                }

                containers.Add(stats);
            }

            if (containers.Count < expectedCount)
            {
                reason = $"Found {containers.Count} out of {expectedCount} body part parameter containers.";
                return false;
            }
            
            result = new Root(rectTransform, containers);
            reason = null;
            return true;
        }

        public sealed class StatsContainer
        {
            public RectTransform RectTransform { get; }
            
            private readonly List<Stat> _stats;

            public StatsContainer(RectTransform rectTransform, List<Stat> stats)
            {
                RectTransform = rectTransform;
                _stats = stats;
            }

            public void MakeStretchable()
            {
                foreach (Stat stat in _stats)
                    stat.MakeStretchable();
                
                RectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
            }

            public static Boolean TryCreate(RectTransform rectTransform, out StatsContainer result, out FormattableString reason)
            {
                result = null;

                const String expectedEnding = "Stats";
                if (!rectTransform.name.EndsWith(expectedEnding))
                {
                    reason = $"Invalid name of object [{rectTransform.name}]. Expected: [*{expectedEnding}].";
                    return false;
                }

                List<Stat> stats = new();
                foreach (RectTransform child in rectTransform.EnumerateChildren())
                {
                    if (child.childCount != 3)
                        continue;
                    
                    if (!Stat.TryCreate(child, out Stat stat, out reason)) 
                    {
                        reason = $"Failed to create [{nameof(Stat)}] object for [{child.name}]. Reason: {reason}.";
                        return false;
                    }

                    stats.Add(stat);
                }

                if (stats.Count < 1)
                {
                    reason = $"No body part parameters could be found..";
                    return false;
                }

                result = new StatsContainer(rectTransform, stats);
                reason = null;
                return true;
            }
        }
        
        public sealed class Stat
        {
            private readonly RectTransform _rectTransform;
            private readonly StatLabel _label;

            public Stat(RectTransform rectTransform, StatLabel label)
            {
                _rectTransform = rectTransform;
                _label = label;
            }

            public void MakeStretchable()
            {
                _label.MakeStretchable();
            }

            public static Boolean TryCreate(RectTransform rectTransform, out Stat result, out FormattableString reason)
            {
                result = null;

                IReadOnlyDictionary<String, RectTransform> childrenByName = rectTransform.GetChildrenByName();
                
                if (!StatLabel.TryCreate(childrenByName, statName: rectTransform.name, out StatLabel label, out reason))
                {
                    reason = $"Failed to create [{nameof(StatLabel)}]. Reason: {reason}";
                    return false;
                }

                result = new Stat(rectTransform, label);
                reason = null;
                return true;
            }
        }
        
        public sealed class StatLabel
        {
            private readonly RectTransform _rectTransform;

            public StatLabel(RectTransform rectTransform)
            {
                _rectTransform = rectTransform;
            }

            public void MakeStretchable()
            {
                _rectTransform.SetAnchors(AnchorPreset.StretchAll, keepCurrentRect: true);
            }

            public static Boolean TryCreate(IReadOnlyDictionary<String, RectTransform> objects, String statName, out StatLabel result, out FormattableString reason)
            {
                result = null;
                
                String expectedName = $"Mask{statName}";
                if (!objects.TryGetValue(expectedName, out RectTransform labelTransform))
                {
                    reason = $"Cannot find object by name [{expectedName}].";
                    return false;
                }

                RectTransform statText = labelTransform.FindChildByName(statName);
                if (statText is null)
                {
                    reason = $"Cannot find object by name [{statName}] inside [{expectedName}].";
                    return false;
                }

                result = new StatLabel(labelTransform);
                reason = null;
                return true;
            }
        }
    }
}