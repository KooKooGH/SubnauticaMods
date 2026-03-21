using System;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using PodshellLeviathan.Mono.Baby;
using UnityEngine;

namespace PodshellLeviathan.Prefabs;

public class PosterPrefab
{
    private static readonly int SpecTex = Shader.PropertyToID("_SpecTex");

    private PrefabInfo Info { get; }

    private readonly Func<Texture2D> _posterImage;

    public PosterPrefab(PrefabInfo info, Func<Texture2D> posterImage)
    {
        Info = info;
        _posterImage = posterImage;
    }

    public void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetEquipment(EquipmentType.Hand);
        prefab.SetGameObject(new CloneTemplate(Info, TechType.PosterKitty)
        {
            ModifyPrefab = obj =>
            {
                var renderers = obj.GetComponentsInChildren<Renderer>();
                var posterMaterial = renderers[0].materials[1];

                var posterImage = _posterImage.Invoke();
                posterMaterial.mainTexture = posterImage;
                posterMaterial.SetTexture(SpecTex, posterImage);

                if (Info.TechType == Plugin.PodshellPosterInfo.TechType)
                {
                    obj.AddComponent<DisablePosterSpawnsOnPickUp>().pickupable = obj.GetComponent<Pickupable>();
                }
            }
        });
        prefab.Register();
    }
}