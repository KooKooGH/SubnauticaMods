using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Kelp;

public class LushCaveBrine
{
    private readonly PrefabInfo _info;
    private readonly string _originalClassId;

    public LushCaveBrine(string classId, string originalClassId, string folderPath)
    {
        _info = PrefabInfo.WithTechType(classId)
            .WithFileName(folderPath + "/" + classId);
        _originalClassId = originalClassId;
    }

    public void Register()
    {
        var prefab = new CustomPrefab(_info);
        prefab.SetGameObject(new CloneTemplate(_info, _originalClassId)
        {
            ModifyPrefab = obj =>
            {
                obj.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.VeryFar;
                var surface = obj.transform.Find("Surface").gameObject.GetComponent<Renderer>();
                var newColor = new Color(0.6f, 0.8f, 1f);
                surface.material.color = newColor;
                var underSurface = obj.transform.Find("UnderSurface").gameObject.GetComponent<Renderer>();
                underSurface.material.color = newColor;
                var trigger = obj.transform.Find("Trigger").gameObject;
                var brineTrigger = trigger.GetComponent<AcidicBrineDamageTrigger>();
                brineTrigger.enabled = false;
                foreach (var atmosphere in obj.GetComponentsInChildren<AtmosphereVolume>())
                {
                    atmosphere.overrideBiome = "lushcave_brine";
                    atmosphere.priority = 65;
                }
            }
        });
        prefab.Register();
    }
}