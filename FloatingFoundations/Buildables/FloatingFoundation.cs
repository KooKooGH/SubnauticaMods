using System.Collections;
using FloatingFoundations.API;
using FloatingFoundations.MonoBehaviours;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using UWE;
using Object = UnityEngine.Object;

namespace FloatingFoundations.Buildables;

public static class FloatingFoundation
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("FloatingFoundation")
        .WithIcon(Plugin.Bundle.LoadAsset<Sprite>("FloatingFoundationIcon"));

    public static void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(BuildPrefab);
        prefab.SetRecipe(new RecipeData(new Ingredient(TechType.Titanium, 2),
            new Ingredient(TechType.Silicone, 1),
            new Ingredient(TechType.Aerogel, 1)
            ));
        prefab.SetPdaGroupCategoryAfter(TechGroup.BasePieces, TechCategory.BasePiece, TechType.BaseFoundation)
            .WithAnalysisTech(Plugin.Bundle.LoadAsset<Sprite>("FloatingFoundationPopup"),
                KnownTechHandler.DefaultUnlockData.BlueprintUnlockSound,
                KnownTechHandler.DefaultUnlockData.BlueprintUnlockMessage);
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
        Object.DestroyImmediate(foundationModel.GetComponentInChildren<TechTag>());

        var pillowPrefab = Plugin.Bundle.LoadAsset<GameObject>("FoundationPillowsPrefab");
        
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
        constructable.placeMaxDistance = 13;
        constructable.allowedUnderwater = false;
        
        var seamothRequest = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);
        yield return seamothRequest;
        var seamothRef = seamothRequest.GetResult();
        
        var seamothProxy = seamothRef.GetComponentInChildren<WaterClipProxy>();
        
        var clipProxy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        clipProxy.name = "WaterClipProxy";
        clipProxy.transform.parent = prefab.transform;
        clipProxy.transform.localScale = new Vector3(9.8f, 1, 9.8f);
        clipProxy.transform.localPosition = new Vector3(0, 0.476f, 0);
        Object.DestroyImmediate(clipProxy.GetComponent<Collider>());
        var waterClip = clipProxy.gameObject.AddComponent<WaterClipProxy>();
        waterClip.shape = WaterClipProxy.Shape.Box;
        waterClip.clipMaterial = seamothProxy.clipMaterial;
        clipProxy.layer = LayerID.BaseClipProxy;
        prefab.AddComponent<FixClipProxyLayer>().clipProxy = clipProxy;

        prefab.AddComponent<DisableClipProxy>().clipProxy = clipProxy;

        var colliderHolder = prefab.transform.Find("BaseFoundationPiece(Clone)/models/BaseFoundationPlatform");
        var newColliderObj = new GameObject("Collider");
        newColliderObj.transform.SetParent(colliderHolder);
        newColliderObj.transform.localScale = Vector3.one;
        newColliderObj.transform.localPosition = Vector3.zero;
        newColliderObj.transform.localRotation = Quaternion.identity;
        var originalCollider = colliderHolder.GetComponent<BoxCollider>();
        var newCollider = newColliderObj.AddComponent<BoxCollider>();
        newCollider.center = originalCollider.center;
        newCollider.size = originalCollider.size;
        newColliderObj.layer = LayerID.TerrainCollider;
        Object.DestroyImmediate(originalCollider);

        result.Set(prefab);
    }
}