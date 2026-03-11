using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using FloatingFoundations.API;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using UWE;
using Object = UnityEngine.Object;

namespace FloatingFoundations.Buildables;

public static class FloatingFoundation
{
    public static PrefabInfo Info { get; private set; } = PrefabInfo.WithTechType("FloatingFoundation");

    public static void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(BuildPrefab);
        prefab.SetRecipe(new RecipeData(new Ingredient(TechType.Titanium, 2),
            new Ingredient(TechType.Silicone, 1),
            new Ingredient(TechType.Floater, 1)
            ));
        prefab.SetPdaGroupCategoryAfter(TechGroup.BasePieces, TechCategory.BasePiece, TechType.BaseFoundation);
        prefab.Register();
        FloatingBuildableUtils.RegisterBuildableAsFloating(Info.TechType, -0.17f);
    }

    private static IEnumerator BuildPrefab(IOut<GameObject> result)
    {
        var request =
            PrefabDatabase.GetPrefabForFilenameAsync("Assets/Prefabs/Base/GeneratorPieces/BaseFoundationPiece.prefab");
        yield return request;
        if (!request.TryGetPrefab(out var baseFoundationPiece))
        {
            Plugin.Logger.LogError("Failed to load prefab with name BaseFoundationPiece!");
            yield break;
        }

        var prefab = new GameObject(Info.ClassID);
        prefab.SetActive(false);
        
        var foundationModel = Object.Instantiate(baseFoundationPiece, prefab.transform);
        foundationModel.transform.localPosition = new Vector3(0, -1, 0);
        foundationModel.transform.localRotation = Quaternion.identity;

        var pillowPrefab = Object.Instantiate(Plugin.Bundle.LoadAsset<GameObject>("FoundationPillowsPrefab"));
        
        var pillows = Object.Instantiate(pillowPrefab, foundationModel.transform);
        pillows.transform.localPosition = Vector3.zero;
        pillows.transform.localRotation = Quaternion.identity;
        
        MaterialUtils.ApplySNShaders(pillows, 6f);
        PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
        var constructable = PrefabUtils.AddConstructable(prefab, Info.TechType,
            ConstructableFlags.Outside | ConstructableFlags.Rotatable, foundationModel);
        constructable.forceUpright = true;
        constructable.placeDefaultDistance = 10;
        constructable.placeMinDistance = 3;
        constructable.placeMaxDistance = 20;
        constructable.allowedUnderwater = false;
        
        var seamothRequest = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);
        yield return seamothRequest;
        var seamothRef = seamothRequest.GetResult();
        
        var seamothProxy = seamothRef.GetComponentInChildren<WaterClipProxy>();
        
        var clipProxy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        clipProxy.transform.parent = prefab.transform;
        clipProxy.transform.localScale = new Vector3(9.8f, 1, 9.8f);
        clipProxy.transform.localPosition = new Vector3(0, 0.476f, 0);
        Object.DestroyImmediate(clipProxy.GetComponent<Collider>());
        var waterClip = clipProxy.gameObject.AddComponent<WaterClipProxy>();
        waterClip.shape = WaterClipProxy.Shape.Box;
        waterClip.clipMaterial = seamothProxy.clipMaterial;
        clipProxy.layer = LayerID.BaseClipProxy;
        prefab.AddComponent<FixClipProxyLayer>().clipProxy = clipProxy;

        result.Set(prefab);
    }

    private class FixClipProxyLayer : MonoBehaviour, IConstructable
    {
        public GameObject clipProxy;
        
        private void Start()
        {
            FixLayer();
        }

        private void FixLayer()
        {
            clipProxy.layer = LayerID.BaseClipProxy;
        }

        public bool IsDeconstructionObstacle()
        {
            return false;
        }

        public bool CanDeconstruct([UnscopedRef] out string reason)
        {
            reason = null;
            return true;
        }

        public void OnConstructedChanged(bool constructed)
        {
            FixLayer();
            StartCoroutine(FixLayerNextFrame());
        }

        private IEnumerator FixLayerNextFrame()
        {
            yield return null;
            FixLayer();
        }
    }
}