using System.Collections;
using IceDragon.Framework.MaterialModifiers;
using Nautilus.Assets;
using Nautilus.Extensions;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace IceDragon.Content.FrozenIceDragon;

public static class FrozenIceDragonPrefab
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("FrozenIceDragon");

    public static void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(GetPrefab);
        prefab.Register();
        
        PDAHandler.AddEncyclopediaEntry("FrozenIceDragon", "Lifeforms/Fauna/Deceased", PDAHandler.UnlockBasic, null, null);
        PDAHandler.AddCustomScannerEntry(Info.TechType, 15, false, "FrozenIceDragon");
    }

    private static IEnumerator GetPrefab(IOut<GameObject> result)
    {
        var prefab = Object.Instantiate(Plugin.Bundle.LoadAsset<GameObject>("FrozenIceDragonPrefab"));
        prefab.SetActive(false);
        PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Far);
        MaterialUtils.ApplySNShaders(prefab, 6, 2, 1f, new IceDragonMaterialModifier());

        var animator = prefab.GetComponentInChildren<Animator>();
        
        var flail = prefab.AddComponent<FrozenDragonFlail>();
        flail.soundPosition = prefab.transform.Find("FlailSoundEmitter");
        flail.sound = AudioUtils.GetFmodAsset("FrozenIceDragonFlail");

        var roar = prefab.AddComponent<FrozenIceDragonRoarOnScan>();
        roar.animator = animator;
        roar.roarPosition = prefab.transform.SearchChild("Head");
        roar.sound = AudioUtils.GetFmodAsset("FrozenIceDragonRoarOnScan");

        prefab.AddComponent<ConstructionObstacle>();
        
        result.Set(prefab);
        yield break;
    }
}