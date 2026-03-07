using System;
using System.Collections;
using KallieʼsPropPack.PrefabLoading;
using Nautilus.Utility.MaterialModifiers;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Precursor;

public class ShatteredArchitectFactory : IEpicPrefabFactory
{
    public IEnumerator BuildVariant(GameObject prefab, LoadedPrefabRegistrationData.Parameter[] parameters)
    {
        prefab.transform.GetChild(0).localScale *= 1.7f;
        yield break;
    }

    public MaterialModifier[] MaterialModifiers => Array.Empty<MaterialModifier>();
}