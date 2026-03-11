using System.Collections;
using ECCLibrary.Data;
using Nautilus.Assets;
using PodshellLeviathan.Mono;
using PodshellLeviathan.Mono.Baby;
using UnityEngine;

namespace PodshellLeviathan.Prefabs;

public class PodshellHatchedBabyPrefab : PodshellLeviathanBabyPrefab
{
    private const float MinDistanceToFollow = 6;
    private const float LeashDistance = 7;
    private const float FollowSwimVelocity = 2.5f;
    
    public PodshellHatchedBabyPrefab(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override CreatureTemplate CreateTemplate()
    {
        var template = base.CreateTemplate();
        template.StayAtLeashData.leashDistance = LeashDistance;
        template.StayAtLeashData.evaluatePriority = BabyStayAtLeashPriority;
        template.StayAtLeashData.swimVelocity = FollowSwimVelocity;
        template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.18f, 0.3f, 0.9f, 2, false, false,
            new CustomGameObjectReference(PrefabInfo.ClassID)));
        template.SetCreatureComponentType<PodshellBabyBehaviour>();
        template.CellLevel = LargeWorldEntity.CellLevel.Far;
        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        yield return base.ModifyPrefab(prefab, components);
        var waterParkCreature = prefab.GetComponent<WaterParkCreature>();
        
        var handTarget = prefab.transform.Find("HandTarget");
        handTarget.gameObject.layer = LayerID.Useable;
        
        var commands = handTarget.gameObject.AddComponent<BabyPodshellCommands>();
        commands.identifier = components.PrefabIdentifier;
        commands.voice = prefab.GetComponent<PodshellVoice>();
        commands.animator = components.Animator;
        commands.waterPark = waterParkCreature;

        var stay = prefab.AddComponent<BabyPodshellStay>();
        stay.commands = commands;
        stay.evaluatePriority = 1;
        stay.rb = components.Rigidbody;

        var followMode = prefab.AddComponent<BabyPodshellTeleport>();
        followMode.commands = commands;

        var followPlayer = prefab.AddComponent<CreatureFollowPlayer>();
        followPlayer.creature = components.Creature;
        followPlayer.distanceToPlayer = MinDistanceToFollow;

        commands.follow = followPlayer;

        var hitGlass = prefab.AddComponent<BabyHitGlassSound>();
        hitGlass.sound = ModAudio.PodshellBabyHitHead;
        hitGlass.waterPark = waterParkCreature;
        hitGlass.rb = components.Rigidbody;

        var behaviour = components.Creature as PodshellBabyBehaviour;
        behaviour.cinematicTarget = handTarget.gameObject;
        behaviour.waterParkCreature = waterParkCreature;
    }
}