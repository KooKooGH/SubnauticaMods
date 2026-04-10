using System;
using System.Collections;
using KallieʼsPropPack.PrefabLoading;
using Nautilus.Utility;
using Nautilus.Utility.MaterialModifiers;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Kelp;

public class CaveRockFactory : IEpicPrefabFactory
{
    public IEnumerator BuildVariant(GameObject prefab, LoadedPrefabRegistrationData.Parameter[] parameters)
    {
        yield break;
    }

    public MaterialModifier[] MaterialModifiers { get; } = {new CaveRockModifier()};

    private class CaveRockModifier : MaterialModifier
    {
        public override void EditMaterial(Material material, Renderer renderer, int materialIndex, MaterialUtils.MaterialType materialType)
        {
            material.color = Color.gray;
            material.SetColor("_SpecColor", Color.gray);
        }
    }
}