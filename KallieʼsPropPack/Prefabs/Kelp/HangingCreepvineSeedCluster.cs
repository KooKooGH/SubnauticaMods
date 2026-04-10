using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Kelp;

public static class HangingCreepvineSeedCluster
{
    private static PrefabInfo Info { get; } = PrefabInfo.WithTechType("HangingCreepvineSeedCluster")
        .WithFileName("KallieʼsPropPack/Kelp/HangingCreepvineSeedCluster");

    public static void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(new CloneTemplate(Info, "e0b1f772-b1bd-43a3-9b56-5f6979aab42d")
        {
            ModifyPrefab = obj =>
            {
                var model = obj.transform.GetChild(0);
                model.localScale *= 4;
                model.localPosition = new Vector3(0.079f, -30.87f, 0.043f);
                Object.DestroyImmediate(obj.GetComponent<Pickupable>());
                Object.DestroyImmediate(obj.GetComponent<WorldForces>());
                Object.DestroyImmediate(obj.GetComponent<Rigidbody>());
            }
        });
        prefab.Register();
    }
}