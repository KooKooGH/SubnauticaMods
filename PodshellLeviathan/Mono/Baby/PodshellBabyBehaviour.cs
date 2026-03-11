using UnityEngine;

namespace PodshellLeviathan.Mono.Baby;

public class PodshellBabyBehaviour : PodshellLeviathanBehavior
{
    public GameObject cinematicTarget;
    public WaterParkCreature waterParkCreature;

    public override void Start()
    {
        base.Start();
        SetFriend(Player.main.gameObject);
    }

    private void OnAddToWaterPark(WaterParkCreature creature)
    {
        cinematicTarget.SetActive(false);
    }

    public override void OnDrop()
    {
        base.OnDrop();
        cinematicTarget.SetActive(!waterParkCreature.IsInsideWaterPark());
    }

}