using System.Collections;
using KallieʼsPropPack.PrefabLoading;
using Nautilus.Utility;
using Nautilus.Utility.MaterialModifiers;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.WiringKit;

public class WiringKitFactory : IEpicPrefabFactory
{
    public IEnumerator BuildVariant(GameObject prefab, LoadedPrefabRegistrationData.Parameter[] parameters)
    {
        yield return MaterialUtils.ReplaceMockMaterials(prefab);
    }

    public MaterialModifier[] MaterialModifiers => System.Array.Empty<MaterialModifier>();
}