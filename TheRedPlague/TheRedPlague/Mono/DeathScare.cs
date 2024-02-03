﻿using System.Collections;
using Nautilus.Utility;
using UnityEngine;

namespace TheRedPlague.Mono;

public class DeathScare : MonoBehaviour
{
    private static FMODAsset _sound = AudioUtils.GetFmodAsset("CloseJumpScare");
    private float yOffset = 0f;
    private float zOffset = 0.5f;
    private static DeathScare current;
    
    public static void PlayDeathScare()
    {
        if (current != null)
            return;
        UWE.CoroutineHost.StartCoroutine(SpawnDeathScare());
    }

    private static IEnumerator SpawnDeathScare()
    {
        GameObject obj;
        if (Random.value < 0.5f)
        {
            obj = Instantiate(Plugin.AssetBundle.LoadAsset<GameObject>("DeathScare"));
            obj.AddComponent<DeathScare>();
        }
        else
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Warper);
            yield return task;
            obj = Instantiate(task.GetResult());
            obj.GetComponent<Creature>().enabled = false;
            var scare = obj.AddComponent<DeathScare>();
            scare.yOffset = -1.4f;
            scare.zOffset = 1.3f;
        }
    }

    private void Awake()
    {
        current = this;
    }
    
    private void Start()
    {
        Invoke(nameof(Kill), 1.5f);
        Utils.PlayFMODAsset(_sound, Player.main.transform.position);
        MainCameraControl.main.ShakeCamera(10, 6, MainCameraControl.ShakeMode.Quadratic, 1.5f);
        FadingOverlay.PlayFX(new Color(0.5f, 0f, 0f), 0.1f, 0.2f, 0.1f, 0.1f);
    }

    private void LateUpdate()
    {
        var camera = MainCamera.camera.transform;
        var offset = camera.forward * zOffset + camera.up * yOffset;
        transform.position = camera.position + offset;
        transform.forward = -camera.forward;
    }

    private void Kill()
    {
        Player.main.liveMixin.TakeDamage(1000);
        FadingOverlay.PlayFX(new Color(0.1f, 0f, 0f), 0.1f, 0.2f, 0.1f);
        Destroy(gameObject, 0.2f);
    }
}