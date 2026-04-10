using UnityEngine;

namespace KallieʼsPropPack.Prefabs.Kelp;

public static class KelpLights
{
    // Intensity
    private const float Dim = 0.7f;
    private const float Normal = 1.8f;
    
    // Size
    private const float Small = 4f;
    private const float Medium = 10f;
    private const float Far = 17f;
    private const float VeryFar = 24f;

    private const string FolderPath = "KallieʼsPropPack/Kelp";
    
    private static readonly Color LightColor = new(1f, 0.8f, 0f);
    private static readonly Color BrineLightColor = new(0.6f, 0.8f, 1f);
    
    private static readonly LightPrefab[] Lights =
    {
        new("KelpLight_Small_Dim", FolderPath, LightColor, Dim, Small),
        new("KelpLight_Medium_Dim", FolderPath, LightColor, Dim, Medium),
        new("KelpLight_Far_Dim", FolderPath, LightColor, Dim, Far),
        new("KelpLight_VeryFar_Dim", FolderPath, LightColor, Dim, VeryFar),
        new("KelpLight_Small_Normal", FolderPath, LightColor, Normal, Small),
        new("KelpLight_Medium_Normal", FolderPath, LightColor, Normal, Medium),
        new("KelpLight_Far_Normal", FolderPath, LightColor, Normal, Far),
        new("KelpLight_VeryFar_Normal", FolderPath, LightColor, Normal, VeryFar),
        new("LushBrineLight_Far_Dim", FolderPath, BrineLightColor, Dim, Far),
        new("LushBrineLight_VeryFar_Dim", FolderPath, BrineLightColor, Dim, VeryFar)
    };
    
    public static void Register()
    {
        foreach (var light in Lights)
        {
            light.Register();
        }
    }
}