using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace PodshellLeviathan.Patches;

[HarmonyPatch(typeof(BaseBioReactor))]
public static class PodshellBioreactorPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(BaseBioReactor.OnAddItem))]
    private static void OnAddItemPostfix(BaseBioReactor __instance, InventoryItem item)
    {
        if (item.item.GetTechType() == Plugin.PodshellLeviathanHatchedBaby.TechType)
        {
            var position = __instance.GetModel().animator.transform.position;
            UWE.CoroutineHost.StartCoroutine(PlayPodshellBioreactorSound(position));
        }
    }

    private static IEnumerator PlayPodshellBioreactorSound(Vector3 position)
    {
        yield return new WaitForSeconds(2);
        FMODUWE.PlayOneShot(ModAudio.PodshellBabyInBioreactor, position);
    }
}