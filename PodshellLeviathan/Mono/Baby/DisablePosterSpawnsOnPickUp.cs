using UnityEngine;

namespace PodshellLeviathan.Mono.Baby;

public class DisablePosterSpawnsOnPickUp : MonoBehaviour
{
    public Pickupable pickupable;

    private void Start()
    {
        pickupable.pickedUpEvent.AddHandler(gameObject, OnPickedUp);
    }

    private void OnPickedUp(Pickupable p)
    {
        Plugin.PickUpPosterGoal.Trigger();
    }
}