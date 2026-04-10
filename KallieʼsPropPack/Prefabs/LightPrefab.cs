using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;

namespace KallieʼsPropPack.Prefabs;

public sealed class LightPrefab
{
    private PrefabInfo Info { get; }
    private Color Color { get; }
    private float Intensity { get; }
    private float Range { get; }
    
    public LightPrefab(string classId, string folderPath, Color color, float intensity, float range)
    {
        Info = PrefabInfo.WithTechType(classId)
            .WithFileName(folderPath + "/" + classId);
        Color = color;
        Intensity = intensity;
        Range = range;
    }

    public void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(GetPrefab);
        prefab.Register();
    }

    private GameObject GetPrefab()
    {
        var obj = new GameObject(Info.ClassID);
        obj.SetActive(false);
        PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, Range > 20 ? LargeWorldEntity.CellLevel.Far : LargeWorldEntity.CellLevel.Near);
        var light = obj.AddComponent<Light>();
        light.color = Color;
        light.intensity = Intensity;
        light.range = Range;
        return obj;
    }
}