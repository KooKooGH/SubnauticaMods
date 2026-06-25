using System.Collections;
using KallieʼsPropPack.MonoBehaviours.VFX;
using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;
using UWE;

namespace KallieʼsPropPack.Prefabs.VFX;

public static class BaseLeakVFX
{
    private static PrefabInfo SprayInfo { get; } =
        new("BaseLeakSprayVFX", "KallieʼsPropPack/VFX/BaseLeakSprayVFX", TechType.None);

    private static PrefabInfo UnderwaterSprayInfo { get; } =
        new("BaseLeakUnderwaterSprayVFX", "KallieʼsPropPack/VFX/BaseLeakUnderwaterSprayVFX", TechType.None);

    private static PrefabInfo RunOnWallInfo { get; } = new("BaseLeakRunOnWallVFX",
        "KallieʼsPropPack/VFX/BaseLeakRunOnWallVFX", TechType.None);

    public static void Register()
    {
        var sprayPrefab = new CustomPrefab(SprayInfo);
        sprayPrefab.SetGameObject(CreateSprayPrefabAsync);
        sprayPrefab.Register();

        var underwaterSprayPrefab = new CustomPrefab(UnderwaterSprayInfo);
        underwaterSprayPrefab.SetGameObject(CreateUnderwaterSprayPrefabAsync);
        underwaterSprayPrefab.Register();

        var runOnWallPrefab = new CustomPrefab(RunOnWallInfo);
        runOnWallPrefab.SetGameObject(CreateRunOnWallPrefabAsync);
        runOnWallPrefab.Register();
    }

    private static IEnumerator CreateUnderwaterSprayPrefabAsync(IOut<GameObject> result)
    {
        var originalResult = new TaskResult<GameObject>();
        yield return CreateSprayPrefabAsync(originalResult);
        var prefab = originalResult.Get();
        prefab.GetComponentInChildren<PlayLeakVFX>().underwater = true;
        prefab.GetComponent<PrefabIdentifier>().ClassId = UnderwaterSprayInfo.ClassID;
        result.Set(prefab);
    }

    private static IEnumerator CreateSprayPrefabAsync(IOut<GameObject> result)
    {
        var request =
            PrefabDatabase.GetPrefabForFilenameAsync("Assets/Prefabs/Base/GeneratorPieces/BaseRoom.prefab");
        yield return request;
        if (!request.TryGetPrefab(out var roomPrefab))
        {
            Plugin.Logger.LogError("Failed to load BaseRoom.prefab");
        }

        var baseLeak = roomPrefab.transform.Find("Flood_BaseRoom/LeakPoints/WallLeakPoint")
            .GetComponent<VFXSubLeakPoint>().leakEffectPrefabs[0];

        var prefab = new GameObject("BaseLeakSprayVFX");
        prefab.SetActive(false);

        GameObject model = Object.Instantiate(baseLeak, prefab.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localEulerAngles = new Vector3(0, 180, 180);

        model.transform.Find("weldPoint").gameObject.SetActive(false);
        
        var vfx = model.EnsureComponent<PlayLeakVFX>();
        vfx.spray = model.GetComponent<VFXWaterSpray>();
        
        Object.DestroyImmediate(model.GetComponent<FMOD_CustomLoopingEmitter>());
        
        foreach (var ps in model.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
        
        PrefabUtils.AddBasicComponents(prefab, SprayInfo.ClassID, TechType.None, LargeWorldEntity.CellLevel.Near);

        result.Set(prefab);
    }

    private static IEnumerator CreateRunOnWallPrefabAsync(IOut<GameObject> result)
    {
        var request =
            PrefabDatabase.GetPrefabForFilenameAsync("Assets/Prefabs/Base/GeneratorPieces/BaseRoom.prefab");
        yield return request;
        if (!request.TryGetPrefab(out var roomPrefab))
        {
            Plugin.Logger.LogError("Failed to load BaseRoom.prefab");
        }

        var vfx = roomPrefab.transform.Find("Flood_BaseRoom/LeakPoints/WallLeakPoint/x_WaterRunOnWall_02").gameObject;

        var prefab = new GameObject("WaterRunOnWallVFX");
        prefab.SetActive(false);
        PrefabUtils.AddBasicComponents(prefab, RunOnWallInfo.ClassID, TechType.None, LargeWorldEntity.CellLevel.Near);

        var model = Object.Instantiate(vfx, prefab.transform, false);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        Object.DestroyImmediate(model.GetComponent<VFXLerpColor>());
        var renderer = model.GetComponent<Renderer>();
        renderer.enabled = true;
        renderer.material.color = renderer.material.color.WithAlpha(0.7f);

        result.Set(prefab);
    }
}