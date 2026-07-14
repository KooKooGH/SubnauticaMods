using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Kelp;

public static class HangingSeaweedFix
{
    public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("HangingSeaweedFixed")
        .WithFileName("KallieʼsPropPack/Kelp/HangingSeaweedFixed");

    public static void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(new CloneTemplate(Info, "6c36eec6-b387-4288-9e94-fce73b7e9d8e")
        {
            ModifyPrefab = obj =>
            {
                var material = obj.GetComponentInChildren<Renderer>().material;
                material.SetFloat("_GlowStrength", 0);
            }
        });
        prefab.Register();
    }
}