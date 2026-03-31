using System.Collections;
using FloatingFoundations.Buildables;
using FloatingFoundations.MonoBehaviours;
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using UWE;

namespace FloatingFoundations.Fragments;

public static class FloatingFoundationFragments
{
    private static TechType FragmentTechType { get; } = EnumHandler.AddEntry<TechType>("FloatingFoundationFragment");
    private static PrefabInfo FloatingFragment { get; } = new("FloatingFoundationFragment_Floating", "FloatingFoundationFragment_Floating", FragmentTechType);
    private static PrefabInfo SunkenFragment { get; } = new("FloatingFoundationFragment_Sunken", "FloatingFoundationFragment_Sunken", FragmentTechType);
    private static PrefabInfo RustedFragment { get; } = new("FloatingFoundationFragment_Rusted", "FloatingFoundationFragment_Rusted", FragmentTechType);

    public static void Register()
    {
        PDAHandler.AddCustomScannerEntry(FragmentTechType, FloatingFoundation.Info.TechType, true, 1, 5f, false);
        
        var floatingPrefab = new CustomPrefab(FloatingFragment);
        floatingPrefab.SetGameObject(CreateFloatingFragmentPrefab);
        floatingPrefab.Register();
        
        var sunkenPrefab = new CustomPrefab(SunkenFragment);
        sunkenPrefab.SetGameObject(CreateSunkenFragmentPrefab);
        sunkenPrefab.Register();
        
        var rustedPrefab = new CustomPrefab(RustedFragment);
        rustedPrefab.SetGameObject(CreateRustedPrefab);
        rustedPrefab.Register();
    }

    
    private static IEnumerator CreateFloatingFragmentPrefab(IOut<GameObject> result)
    {
        yield return CreatePrefab(result, true);
    }
    
    private static IEnumerator CreateSunkenFragmentPrefab(IOut<GameObject> result)
    {
        yield return CreatePrefab(result, false);
    }

    private static IEnumerator CreatePrefab(IOut<GameObject> result, bool useClipProxy)
    {
        var request =
            PrefabDatabase.GetPrefabForFilenameAsync("Assets/Prefabs/Base/GeneratorPieces/BaseFoundationPiece.prefab");
        yield return request;
        if (!request.TryGetPrefab(out var baseFoundationPiece))
        {
            Plugin.Logger.LogError("Failed to load prefab with name BaseFoundationPiece!");
            yield break;
        }

        var prefab = new GameObject();
        prefab.SetActive(false);
        
        var foundationModel = Object.Instantiate(baseFoundationPiece, prefab.transform);
        foundationModel.transform.localPosition = new Vector3(0, -1, 0);
        foundationModel.transform.localRotation = Quaternion.identity;
        Object.DestroyImmediate(foundationModel.GetComponentInChildren<TechTag>());
        foundationModel.SetActive(true);

        var pillowPrefab = Plugin.Bundle.LoadAsset<GameObject>("FoundationPillowsPrefab");
        
        var pillows = Object.Instantiate(pillowPrefab, foundationModel.transform);
        pillows.transform.localPosition = Vector3.zero;
        pillows.transform.localRotation = Quaternion.identity;
        
        MaterialUtils.ApplySNShaders(pillows, 6f);
        PrefabUtils.AddBasicComponents(prefab, FloatingFragment.ClassID, FragmentTechType, LargeWorldEntity.CellLevel.Far);
        
        if (useClipProxy)
        {
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
        }
        
        result.Set(prefab);
    }
    
    private static IEnumerator CreateRustedPrefab(IOut<GameObject> result)
    {
        var request = PrefabDatabase.GetPrefabAsync("255ed3c3-1973-40c0-9917-d16dd9a7018d");
        yield return request;
        if (!request.TryGetPrefab(out var referenceModel))
        {
            Plugin.Logger.LogError("Failed to load fragment prefab!");
            yield break;
        }

        referenceModel =
            referenceModel.transform.Find("BaseCell/BaseAbandonedFoundationPiece/models/BaseFoundationPlatform").gameObject;

        var prefab = new GameObject();
        prefab.SetActive(false);
        
        var foundationModel = Object.Instantiate(referenceModel, prefab.transform);
        foundationModel.transform.localPosition = new Vector3(0, 0, 0);
        foundationModel.transform.localEulerAngles = new Vector3(270, 180, 0);
        Object.DestroyImmediate(foundationModel.GetComponentInChildren<TechTag>());
        foundationModel.SetActive(true);

        var pillowPrefab = Plugin.Bundle.LoadAsset<GameObject>("FoundationPillowsPrefab");
        
        var pillows = Object.Instantiate(pillowPrefab, foundationModel.transform);
        pillows.transform.localPosition = Vector3.zero;
        pillows.transform.localEulerAngles = new Vector3(90, 0, 0);
        
        MaterialUtils.ApplySNShaders(pillows, 6f);
        PrefabUtils.AddBasicComponents(prefab, FloatingFragment.ClassID, FragmentTechType, LargeWorldEntity.CellLevel.Medium);

        var collider = prefab.AddComponent<BoxCollider>();
        collider.size = new Vector3(10, 0.4f, 10);
        collider.center = new Vector3(0, 1.17f, 0);
        prefab.AddComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.metal;  
        
        result.Set(prefab);
    }
}