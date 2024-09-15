using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Memoria.FrontMission2.BeepInEx;
using Memoria.FrontMission2.Shared.Framework;
using Memoria.FrontMission2.Shared.Framework.Unity;
using UnityEngine;
using UnityEngine.Events;
using Object = System.Object;

namespace Memoria.FrontMission2.HarmonyHooks.AttackRange;

[HarmonyPatch(typeof(HighlightManager), "RedrawHighlights")]
public static class HighlightManager_RedrawHighlights
{
    private static void Postfix(
        HighlightManager __instance,
        Tile[] ___moveRangeTiles,
        Tile[] ___pathTiles,
        Tile[] ___attackRangeTiles,
        BattleUnit ___battleUnit)
    {
        if (!ModComponent.Instance.Config.Battlefield.DisplayAttackRangeOnCell)
            return;

        Cursor cursor = Cursor.Instance;
        BattleUnit activeUnit = ___battleUnit;

        HighlightController controller = cursor.gameObject.EnsureComponent<HighlightController>(initialEnabled: false);
        HighlightControllerInput controllerInput = cursor.gameObject.EnsureComponent<HighlightControllerInput>();
        Tile selectedTile = cursor.SelectedTile;
        
        controller.ChangeTarget(activeUnit, selectedTile);
        controllerInput.Update();
    }

    private sealed class HighlightControllerInput : MonoBehaviour
    {
        private HighlightController _controller;

        public void Start()
        {
            _controller = this.gameObject.EnsureComponent<HighlightController>();
        }

        public void Update()
        {
            if (!ModComponent.Instance.Config.Battlefield.DisplayAttackRangeOnCell)
            {
                _controller.DisposeColorableTiles();
                Destroy(_controller);
                Destroy(this);
                return;
            }

            UpdateInput();
        }

        private void UpdateInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                _controller.ChangeActivity(isActive: true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                _controller.ChangeActivity(isActive: false);
            }
        }
    }
    
    private sealed class HighlightController : MonoBehaviour
    {
        [CanBeNull] private BattleUnit _activeUnit;
        [CanBeNull] private Tile _selectedTile;

        private IReadOnlyList<MemoriaColoredTile> _coloredTiles = Array.Empty<MemoriaColoredTile>();
        
        public void ChangeTarget(BattleUnit activeUnit, Tile selectedTile)
        {
            if (activeUnit == _activeUnit && selectedTile == _selectedTile)
                return;
            
            _activeUnit = activeUnit;
            _selectedTile = selectedTile;
            
            if (!this.enabled)
                return;

            InitializeColorableTiles();
        }

        public void ChangeActivity(Boolean isActive)
        {
            BattleUnit activeUnit = _activeUnit;
            Tile selectedTile = _selectedTile;
            if (activeUnit is null || selectedTile is null)
                return;

            this.enabled = isActive;
        }

        public void OnEnable()
        {
            InitializeColorableTiles();
        }

        public void OnDisable()
        {
            DisposeColorableTiles();
        }

        private void InitializeColorableTiles()
        {
            DisposeColorableTiles();

            if (_activeUnit is null || _selectedTile is null)
                return;
            
            _coloredTiles = MemoriaHighlightManager.GetAttackRange(_activeUnit, _selectedTile);

            if (this.enabled)
                ApplyColors();
        }

        public void DisposeColorableTiles()
        {
            RevertColors();
            _coloredTiles = Array.Empty<MemoriaColoredTile>();
        }

        private void ApplyColors()
        {
            foreach (MemoriaColoredTile tile in _coloredTiles)
                tile.ApplyColor();
        }

        private void RevertColors()
        {
            foreach (MemoriaColoredTile tile in _coloredTiles.Reverse())
                tile.RevertColor();
        }
    }
}

public static class ExtensionMethodsTile
{
    public static void ChangeHighlight(this Tile tile, Color color)
    {
        HighlightType highlightType = (HighlightType)ColorHelper.EncodeColor(color);
        if (highlightType > HighlightType.None && highlightType <= HighlightType.Basic)
            throw new ArgumentException(color.ToString(), nameof(color));
        
        tile.ChangeHighlight(highlightType);
    }
}

public sealed class MemoriaColoredTile
{
    public Tile Tile { get; }
    public HighlightType NewColor { get; }
    public Color? OriginalColor { get; }

    public MemoriaColoredTile(Tile tile, HighlightType newColor)
    {
        Tile = tile;
        NewColor = newColor;

        if (tile.highlightRenderer.enabled)
        {
            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            tile.highlightRenderer.GetPropertyBlock(properties);
            OriginalColor = properties.GetColor("_Tint");
        }
    }

    public void ApplyColor()
    {
        Tile.ChangeHighlight(NewColor);
    }

    public void RevertColor()
    {
        if (OriginalColor is null)
            Tile.ChangeHighlight(HighlightType.None);
        else
            Tile.ChangeHighlight(OriginalColor.Value);
    }
}

public static class MemoriaWeaponHelper
{
    public static IGrouping<Int32, Weapon>[] GetAvailableWeaponOrderedByRange(BattleUnit battleUnit)
    {
        return battleUnit.Machine
            .ReturnWeapons()
            .Where(w => IsWeaponAvailable(w, battleUnit))
            .GroupBy(w => w.Stats.GetMaxRange())
            .OrderBy(g => g.Key)
            .ToArray();
    }
    
    public static Int32 GetAttackApCost(WeaponRange weaponRange, Tile attackerTile, Tile targetTile)
    {
        const Int32 meleeApCost = 0;
        const Int32 shortApCost = 2;
        const Int32 longApCost = 6;
        
        switch (weaponRange)
        {
            case WeaponRange.Melee:
                return meleeApCost;
            case WeaponRange.ShortRange:
                return shortApCost;
            case WeaponRange.LongRange:
                return longApCost;
            case WeaponRange.Bifocal:
                Int32 distance = Grid.GetManhattanDistance(attackerTile, targetTile);
                return distance > 1 ? longApCost : shortApCost;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static Boolean IsWeaponAvailable(Weapon weapon, BattleUnit currentBattleUnit)
    {
        if (AnomalyBlock(currentBattleUnit))
            return false;

        if (weapon.Stats.OwnerPart.Stats.Disabled)
            return false;

        if (!weapon.HasBullets(1))
            return false;

        return true;

        static Boolean AnomalyBlock(BattleUnit owner)
        {
            StatusAnomaly currentAnomaly = owner.StatusAnomaliesHandler.CurrentAnomaly;
            return currentAnomaly is Terror || currentAnomaly is Stun;
        }
    }
}

public static class MemoriaHighlightManager
{
    private static readonly AccessTools.FieldRef<HighlightManager, Tile[]> _moveRangeTiles = AccessTools.FieldRefAccess<HighlightManager, Tile[]>(AccessTools.Field(typeof(HighlightManager), "moveRangeTiles"));
    
    public static IReadOnlyList<MemoriaColoredTile> GetAttackRange(BattleUnit unit, Tile unitPosition)
    {
        Tile[] moveRangeTiles = _moveRangeTiles(BattleTeams.Instance.ActiveUnit.HighlightManager);
        // if (moveRangeTiles.Length == 0)
        //     return Array.Empty<MemoriaColoredTile>(); // To avoid problems during attack phase
        
        IGrouping<Int32, Weapon>[] weaponsByMaxRange = MemoriaWeaponHelper.GetAvailableWeaponOrderedByRange(unit);

        if (weaponsByMaxRange.Length == 0)
            return Array.Empty<MemoriaColoredTile>();

        List<MemoriaColoredTile> result = new(capacity: moveRangeTiles.Length * 2);
        foreach (Tile moveTile in moveRangeTiles)
            result.Add(new MemoriaColoredTile(moveTile, HighlightType.None));

        using (FreezeCurrentAP(unit))
        {
            Int32 currentAP = unit.Pilot.Stats.CurrentAp;

            Int32 movementApCost = Int32.MaxValue;
            if (unit.MovementController.OccupiedTile == unitPosition)
                movementApCost = 0;
            else if (unit.GridPathfinder.CanMoveTo(unitPosition))
                movementApCost = unit.GridPathfinder.GetPathTo(unitPosition).Length - 1;

            // Enumerate in reverse order to show weapon with lesser attack range overlapped by weapon with larger attack range.
            foreach (IGrouping<Int32, Weapon> weaponsToCheck in weaponsByMaxRange.Reverse())
            {
                Int32 weaponMaxRange = weaponsToCheck.Key;
                foreach (Weapon weapon in weaponsToCheck)
                {
                    WeaponRange weaponRange = weapon.Stats.GetRangeType();
                    Int32 additionalMaxRange = (weapon.Stats.WeaponType.Type == WeaponTypes.GrenadeLauncher || weapon.Stats.WeaponType.Type == WeaponTypes.ClusterBomb) ? 1 : 0;

                    unit.Pilot.Stats.CurrentAp = unit.Pilot.Stats.MaxAp;
                    Tile[] tilesForMaxAp = unit.GridPathfinder.GetAttackRange(unitPosition, weapon, additionalMaxRange);
                    
                    unit.Pilot.Stats.CurrentAp = currentAP - movementApCost;
                    Tile[] tiles = unit.GridPathfinder.GetAttackRange(unitPosition, weapon, additionalMaxRange);
                    
                    IEnumerable<Tile> unavailableTiles = tilesForMaxAp.Except(tiles);
                    foreach (Tile attackTile in unavailableTiles)
                        result.Add(new MemoriaColoredTile(attackTile, MemoriaHighlightTypes.NotEnoughAP));
                    
                    foreach (Tile attackTile in tiles)
                    {
                        Int32 attackApCost = MemoriaWeaponHelper.GetAttackApCost(weaponRange, unitPosition, attackTile);
                        if (attackApCost > unit.Pilot.Stats.CurrentAp)
                        {
                            result.Add(new MemoriaColoredTile(attackTile, MemoriaHighlightTypes.NotEnoughAP));
                        }
                        else
                        {
                            HighlightType color = MemoriaHighlightTypes.GetByMaxRange(weaponMaxRange);
                            result.Add(new MemoriaColoredTile(attackTile, color));
                        }
                    }
                }
            }
        }

        return result;
    }

    private static IDisposable FreezeCurrentAP(BattleUnit unit)
    {
        UnityAction<String, Object> onStatChanged = unit.Pilot.Stats.OnStatChanged;
        Int32 previousAP = unit.Pilot.Stats.CurrentAp;

        DisposableAction action = new(() =>
        {
            unit.Pilot.Stats.CurrentAp = previousAP;
            unit.Pilot.Stats.OnStatChanged = onStatChanged;
        });

        unit.Pilot.Stats.OnStatChanged = null;
        return action;
    }
}


public static class MemoriaHighlightTypes
{
    public static HighlightType NotEnoughAP = (HighlightType)ColorHelper.EncodeColor(new Color(0.5f, 0.5f, 0.5f, 0.25f));
    public static HighlightType Melee = (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0f, 0f, 1f));
    public static HighlightType Short = (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0.764f, 0f, 1f));
    public static HighlightType Long = (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0f, 0.764f, 1f));
    
    public static HighlightType[] AttackRangeColors { get; } =
    [
        (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0f, 0f, 1f)),
        (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0.4f, 0.15f, 1f)),
        (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0.764f, 0.1f, 1f)),
        (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0.1f, 0.764f, 1f)),
        (HighlightType)ColorHelper.EncodeColor(new Color(0.764f, 0.5f, 0.764f, 1f))
    ];

    public static HighlightType GetByMaxRange(Int32 maxRange)
    {
        Int32 colorIndex = (maxRange - 1) % AttackRangeColors.Length;
        return AttackRangeColors[colorIndex];
    }
    
    public static HighlightType GetByApCost(Int32 attackApCost)
    {
        return attackApCost switch
        {
            0 => Melee,
            2 => Short,
            6 => Long,
            _ => throw new NotSupportedException(attackApCost.ToString())
        };
    }
}