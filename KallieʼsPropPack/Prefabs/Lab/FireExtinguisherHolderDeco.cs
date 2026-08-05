using System.Collections;
using KallieʼsPropPack.Utility;
using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Lab;

public static class FireExtinguisherHolderDeco
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("FireExtinguisherHolderDeco")
        .WithFileName("KallieʼsPropPack/Lab/FireExtinguisherHolderDeco");

    private const string HolderClassId = "80122eca-8265-484a-b4ae-0780a3e5d9cb";
    private const string ExtinguisherClassId = "be2baa90-52b3-46d6-992d-5a2614f36af5";

    public static void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(GetPrefab);
        prefab.Register();
    }

    private static IEnumerator GetPrefab(IOut<GameObject> result)
    {
        var prefab = new GameObject("FireExtinguisherHolder");
        prefab.SetActive(false);
        
        TaskResult<GameObject> holderResult = new TaskResult<GameObject>();
        TaskResult<GameObject> extinguisherResult = new TaskResult<GameObject>();
        
        yield return ChildPrefabUtils.AddChildPrefab(prefab, HolderClassId, holderResult);
        yield return ChildPrefabUtils.AddChildPrefab(prefab, ExtinguisherClassId, extinguisherResult);

        var holder = holderResult.Get();
        var extinguisher = extinguisherResult.Get();
        
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localEulerAngles = new Vector3(0, 90, 90);
        extinguisher.transform.localPosition = new Vector3(0, -0.45f, 0.096f);
        extinguisher.transform.localEulerAngles = new Vector3(355, 180, 0);
        
        // fix holder rendering
        var holderMaterial = holder.GetComponentInChildren<Renderer>().material;
        holderMaterial.SetFloat("_EmissionLM", 0);
        holderMaterial.SetFloat("_EmissionLMNight", 0);
        
        // clean up components
        Object.DestroyImmediate(extinguisher.GetComponent<FireExtinguisher>());
        Object.DestroyImmediate(extinguisher.GetComponent<FMODASRPlayer>());
        Object.DestroyImmediate(extinguisher.GetComponent<FMOD_CustomEmitter>());
        Object.DestroyImmediate(extinguisher.GetComponent<FMOD_StudioEventEmitter>());
        Object.DestroyImmediate(extinguisher.GetComponent<FMOD_CustomLoopingEmitter>());
        // disable vfx
        extinguisher.transform.Find("MaterialEmitter").gameObject.SetActive(false);
        // enable world model
        extinguisher.transform.Find("fire_extinguisher_01_fp").gameObject.SetActive(false);
        extinguisher.transform.Find("fire_extinguisher_01_tp").gameObject.SetActive(true);
        
        PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Near);
        
        result.Set(prefab);
    }
}