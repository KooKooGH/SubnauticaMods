using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PodshellLeviathan.Mono.Baby;

public class BabyPodshellTeleport : MonoBehaviour
{
    public BabyPodshellCommands commands;
    public float warpDistance = 100f;
    public float warpInterval = 5f;

    private void Start()
    {
        InvokeRepeating(nameof(WarpToPlayer), Random.value * warpInterval, warpInterval);
    }

    private bool GetIsFollowingPlayer()
    {
        return commands.GetCurrentState() == BabyPodshellCommands.State.Following;
    }

    private void WarpToPlayer()
    {
        if (!GetIsFollowingPlayer() || Player.main.GetBiomeString().StartsWith("precursor", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        var difference = Player.main.transform.position - transform.position;
        if (!(difference.magnitude > warpDistance))
        {
            return;
        }
        
        var position = Player.main.transform.position - difference.normalized * warpDistance;
        position.y = Mathf.Min(position.y, Ocean.GetOceanLevel() - 1f);
        int hits = UWE.Utils.OverlapSphereIntoSharedBuffer(transform.position, 5f);
        for (int i = 0; i < hits; i++)
        {
            if (UWE.Utils.sharedColliderBuffer[i].GetComponentInParent<SubRoot>() != null)
            {
                return;
            }
        }
        transform.position = position;
    }
}