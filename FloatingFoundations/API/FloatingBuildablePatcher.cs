using System.Collections.Generic;
using HarmonyLib;
using Nautilus.Extensions;
using UnityEngine;

namespace FloatingFoundations.API;

[HarmonyPatch]
public static class FloatingBuildablePatcher
{
    private const float BaseDetectionRange = 60;
    
    internal static void RegisterManualPatches(Harmony harmony)
    {
        var original = AccessTools.Method(typeof(Builder), nameof(Builder.HasComponent),
            new[] { typeof(GameObject) }, new[] { typeof(Constructable) });
        var postfix = new HarmonyMethod(typeof(FloatingBuildablePatcher), nameof(AllowConstructablesOnFloatingFoundations));
        harmony.Patch(original, postfix: postfix);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Builder), nameof(Builder.CheckAsSubModule))]
    public static void AllowPlacementOnWaterSurfacePostfix(ref bool __result)
    {
        if (__result || !FloatingBuildableUtils.FloatingTechTypes.Contains(Builder.constructableTechType)) return;

        var aimTransform = Builder.GetAimTransform();
        var originalPosition = aimTransform.position + aimTransform.forward * Builder.placeDefaultDistance;
        
        bool snapToWaterSurface = Mathf.Abs(originalPosition.y - FloatingBuildableUtils.GetSeaLevelForTechType(Builder.constructableTechType)) < Builder.placeDefaultDistance / 2;

        if (snapToWaterSurface)
        {
            if (string.IsNullOrEmpty(GameInput.GetBinding(GameInput.PrimaryDevice, Plugin.SnapToBasesButton, GameInput.BindingSet.Primary)))
            {
                ErrorMessage.main.AddHint(Language.main.GetFormat("FloatingFoundationSnapUnboundHint"));
            }
            else
            {
                ErrorMessage.main.AddHint(LanguageCache.GetButtonFormat("FloatingFoundationSnapHint", Plugin.SnapToBasesButton));
            }
            __result = true;
        }
    }

    // postfix of Builder.HasComponent<Constructable>
    private static void AllowConstructablesOnFloatingFoundations(GameObject go, ref bool __result)
    {
        var techType = CraftData.GetTechType(go);
        if (FloatingBuildableUtils.FloatingTechTypes.Contains(techType))
        {
            __result = false;
        }
    }

    private static void GetPlacedPosition(float seaLevel, ref Vector3 position, ref Quaternion rotation)
    {
        var aimTransform = Builder.GetAimTransform();
        position = aimTransform.position + aimTransform.forward * Builder.placeDefaultDistance;
        bool snapToWaterSurface = Mathf.Abs(position.y - FloatingBuildableUtils.GetSeaLevelForTechType(Builder.constructableTechType)) < Builder.placeDefaultDistance / 2;
        if (!snapToWaterSurface)
        {
            return;
        }
        var finalPosition = position;
        var targetBase = BaseGhost.FindBase(MainCamera.camera.transform, BaseDetectionRange);
        if (targetBase == null) targetBase = CheckForBasesLazy(MainCamera.camera.transform.position);
        finalPosition = SnapPlacement(new Vector3(finalPosition.x, seaLevel, finalPosition.z), targetBase);
        position = new Vector3(finalPosition.x, seaLevel, finalPosition.z);
        if (GameInput.GetButtonHeld(Plugin.SnapToBasesButton))
        {
            if (targetBase != null)
            {
                rotation = targetBase.transform.rotation;
            }
            else
            {
                rotation = Quaternion.identity;
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Builder), nameof(Builder.SetDefaultPlaceTransform))]
    private static void SetDefaultPlaceTransformPostfix(ref Vector3 position, ref Quaternion rotation)
    {
        if (!FloatingBuildableUtils.FloatingTechTypes.Contains(Builder.constructableTechType)) return;
        GetPlacedPosition(FloatingBuildableUtils.GetSeaLevelForTechType(Builder.constructableTechType), ref position, ref rotation);
    }
    
    private static Vector3 SnapPlacement(Vector3 position, Base targetBase)
    {
        bool snap = GameInput.GetButtonHeld(Plugin.SnapToBasesButton);
        if (!snap)
            return position;
        
        if (targetBase == null)
        {
            var size = Base.cellSize;
            return new Vector3(Mathf.RoundToInt(position.x / size.x) * size.x, position.y, Mathf.RoundToInt(position.z / size.z) * size.z);
        }
        
        var cell = targetBase.WorldToGrid(position);
        var snapped = SnapToCell(cell, targetBase);
        return targetBase.GridToWorld(snapped);
    }
    
    private static readonly Dictionary<Int3, int> Score = new();
    
    private static Int3 SnapToCell(Int3 cell, Base targetBase)
    {
        var cellType = Base.CellType.Foundation;
        Int3 @int = Base.CellSize[(uint)cellType];
        Score.Clear();
        Int3 adjacent = Base.GetAdjacent(cell, Base.Direction.Above);
        int value;
        foreach (Int3 item in Int3.Range(adjacent, adjacent + @int - 1))
        {
            if (targetBase.GetCell(item) == cellType)
            {
                Int3 key = targetBase.NormalizeCell(item);
                if (Score.TryGetValue(key, out value))
                {
                    Score[key] = value + 1;
                }
                else
                {
                    Score.Add(key, 1);
                }
            }
        }
        Int3 adjacent2 = Base.GetAdjacent(cell, Base.Direction.Below);
        foreach (Int3 item2 in Int3.Range(adjacent2, adjacent2 + @int - 1))
        {
            if (targetBase.GetCell(item2) == cellType)
            {
                Int3 key2 = targetBase.NormalizeCell(item2);
                if (Score.TryGetValue(key2, out value))
                {
                    Score[key2] = value + 1;
                }
                else
                {
                    Score.Add(key2, 1);
                }
            }
        }
        int num = 0;
        foreach (KeyValuePair<Int3, int> item3 in Score)
        {
            if (item3.Value > num)
            {
                num = item3.Value;
                cell.x = item3.Key.x;
                cell.z = item3.Key.z;
            }
        }
        return cell;
    }

    private static readonly List<Base> BaseSearchCache = new();
    private static float _timeSearchBasesAgain;
    private const float BaseSearchInterval = 5f;
    private const int MaxGlobalObjectsToScan = 1000;
    
    private static Base CheckForBasesLazy(Vector3 fromPosition)
    {
        if (Time.time > _timeSearchBasesAgain)
        {
            _timeSearchBasesAgain = Time.time + BaseSearchInterval;
            BaseSearchCache.Clear();
            var globalRoot = LargeWorld.main.streamer.globalRoot;
            if (globalRoot == null) return null;
            int i = 0;
            foreach (Transform child in globalRoot.transform)
            {
                i++;

                if (i > MaxGlobalObjectsToScan)
                    break;

                if (child.gameObject.name != "Base(Clone)")
                {
                    continue;
                }

                var baseComponent = child.GetComponent<Base>();
                if (baseComponent != null)
                {
                    BaseSearchCache.Add(baseComponent);
                }
            }
        }

        if (BaseSearchCache.Count == 0)
            return null;
        
        return GetBestBaseChoice(fromPosition);
    }

    private static Base GetBestBaseChoice(Vector3 position)
    {
        float maxDistanceSqr = BaseDetectionRange * BaseDetectionRange;
        Base best = null;
        foreach (var @base in BaseSearchCache)
        {
            if (@base == null)
            {
                continue;
            }

            var distance = Vector3.SqrMagnitude(position - @base.GetClosestPoint(position));
            if (distance < maxDistanceSqr)
            {
                maxDistanceSqr = distance;
                best = @base;
            }
        }

        return best;
    }
}