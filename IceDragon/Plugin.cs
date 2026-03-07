using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using IceDragon.Content.FrozenIceDragon;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace IceDragon;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    
    public static AssetBundle Bundle { get; private set; }

    private static bool _loadedAssets;

    private void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        WaitScreenHandler.RegisterEarlyAsyncLoadTask(PluginInfo.PLUGIN_NAME, LoadTask, "Loading ice dragon...");
            
        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        
        LanguageHandler.RegisterLocalizationFolder();
    }

    private IEnumerator LoadTask(WaitScreenHandler.WaitScreenTask task)
    {
        if (_loadedAssets)
            yield break;
        _loadedAssets = true;
        
        // LOAD
        Bundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly, "icedragon");
        
        // Prefabs
        FrozenIceDragonPrefab.Register();
        
        // Audio
        ModAudio.RegisterAudio();
        
        // Story
        ModStory.Register();
    }
}