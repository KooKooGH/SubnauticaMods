using System.Collections;
using KallieʼsPropPack.MaterialModifiers;
using KallieʼsPropPack.MonoBehaviours.Ice;
using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Ice;

public class ShatterableIce
{
    private PrefabInfo Info { get; }
    private string PrefabName { get; }

    public ShatterableIce(string id, string prefabName)
    {
        Info = PrefabInfo.WithTechType("Kallies_" + id)
            .WithFileName("KallieʼsPropPack/Ice/" + id);
        PrefabName = prefabName;
    }

    public void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(GetGameObject);
        prefab.Register();
    }

    private IEnumerator GetGameObject(IOut<GameObject> result)
    {
        var prefab = Object.Instantiate(Plugin.Bundle.LoadAsset<GameObject>(PrefabName));
        PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);
        var solidParent = prefab.transform.Find("Solid");
        var shatteredParent = prefab.transform.Find("Shattered");
        foreach (var meshRenderer in prefab.GetComponentsInChildren<MeshRenderer>(true))
        {
            var material = meshRenderer.material;
            var materialType = meshRenderer.transform.parent == shatteredParent
                ? MaterialUtils.MaterialType.Transparent
                : MaterialUtils.MaterialType.Opaque;
            MaterialUtils.ApplyUBERShader(material, 4, 50, 0.2f, materialType);

            var modifier = new IceMaterialModifier(false);
            modifier.EditMaterial(material, meshRenderer, 0, materialType);
        }

        foreach (var rb in shatteredParent.GetComponentsInChildren<Rigidbody>())
        {
            var wf = rb.gameObject.AddComponent<WorldForces>();
            wf.useRigidbody = rb;
            wf.underwaterGravity = 0.5f;
        }

        var behaviour = prefab.AddComponent<ShatterableIceBehaviour>();
        behaviour.normalVersion = solidParent.gameObject;
        behaviour.shatteredVersion = shatteredParent.gameObject;

        result.Set(prefab);
        yield break;
    }
}