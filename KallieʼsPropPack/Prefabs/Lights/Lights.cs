using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;
using UWE;

namespace KallieʼsPropPack.Prefabs.Lights;

public class CustomLight
{
    private PrefabInfo Info { get; }

    private Color LightColor { get; set; } = Color.white;
    private float Intensity { get; set; } = 1f;
    private float Range { get; set; } = 10f;

    public CustomLight(string classId)
    {
        Info = PrefabInfo.WithTechType(classId);
    }

    public CustomLight WithColor(Color color)
    {
        LightColor = color;
        return this;
    }

    public CustomLight WithIntensity(float intensity)
    {
        Intensity = intensity;
        return this;
    }

    public CustomLight WithRange(float range)
    {
        Range = range;
        return this;
    }

    public void Register()
    {
        var prefab = new CustomPrefab(Info);
        prefab.SetGameObject(() =>
        {
            var lightPrefab = new GameObject(Info.ClassID);
            lightPrefab.SetActive(false);

            PrefabUtils.AddBasicComponents(lightPrefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Near);

            var pointLight = lightPrefab.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.intensity = Intensity;
            pointLight.color = LightColor;
            pointLight.range = Range;

            return lightPrefab;
        });
        prefab.Register();
    }
}