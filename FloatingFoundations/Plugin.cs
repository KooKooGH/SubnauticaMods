using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using FloatingFoundations.Buildables;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace FloatingFoundations;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    
    internal static AssetBundle Bundle { get; private set; }

    private static bool _loadedAssets;

    private void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        LanguageHandler.RegisterLocalizationFolder();
        
        WaitScreenHandler.RegisterEarlyLoadTask(PluginInfo.PLUGIN_NAME, RegisterMod);

        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private static void RegisterMod(WaitScreenHandler.WaitScreenTask task)
    {
        if (_loadedAssets) return;
        
        Bundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly, "floatingfoundation");
        FloatingFoundation.Register();
        
        _loadedAssets = true;
    }
}