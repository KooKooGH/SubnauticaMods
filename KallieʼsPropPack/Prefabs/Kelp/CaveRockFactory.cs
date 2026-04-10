using System;
using System.Collections;
using KallieʼsPropPack.PrefabLoading;
using Nautilus.Utility.MaterialModifiers;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Kelp;

public class CaveRockFactory : IEpicPrefabFactory
{
    public IEnumerator BuildVariant(GameObject prefab, LoadedPrefabRegistrationData.Parameter[] parameters)
    {
        yield break;
    }

    public MaterialModifier[] MaterialModifiers { get; } = Array.Empty<MaterialModifier>();
}