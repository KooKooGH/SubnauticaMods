using System.Collections;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Utility.MaterialModifiers;
using PodshellLeviathan.Mono.Baby;
using UnityEngine;

namespace PodshellLeviathan.Prefabs;

public class PodshellLeviathanBabyPrefab : PodshellLeviathanPrefab
{
    public PodshellLeviathanBabyPrefab(PrefabInfo prefabInfo) : base(prefabInfo)
    {
    }

    protected override float StandardSwimVelocity => 1.8f;
    protected override string ModelName => "PodshellLeviathanBabyPrefab";
    protected override float MaxHealth => 5000;
    protected override float Mass => 200;
    protected override bool UseScreenShake => false;
    protected override MaterialModifier[] MaterialModifiers =>  new MaterialModifier[] { new PodshellMaterialModifier(true ) };
    protected override FMODAsset LongRoarClose => ModAudio.PodshellBabyRoar;
    protected override FMODAsset LongRoarFar => ModAudio.PodshellBabyRoar;
    protected override FMODAsset ShortRoarClose => ModAudio.PodshellBabyRoar;
    protected override FMODAsset ShortRoarFar => ModAudio.PodshellBabyRoar;
    protected override FMODAsset DeathSound => ModAudio.PodshellBabyDeath;
    protected override bool UseBabySounds => true;
    protected override bool TriggerIntroductionGoal => false;

    protected override ShellFragmentSettings FragmentSettings => new(false, 0.3f);

    protected override CreatureTemplate CreateTemplate()
    {
        var template = base.CreateTemplate();
        template.BioReactorCharge = 650;
        template.AvoidObstaclesData =
            new AvoidObstaclesData(AvoidObstaclesPriority, StandardSwimVelocity, false, 7, 8);
        template.LocomotionData.forwardRotationSpeed = 0.2f;
        template.CellLevel = LargeWorldEntity.CellLevel.Medium;
        template.AvoidTerrainData = null;
        return template;
    }

    protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        yield return base.ModifyPrefab(prefab, components);
        var head = prefab.transform.Find("turtle_rigged/DO_NOT_TOUCH/root/cog/neck1");
        prefab.AddComponent<PodshellBabyHeadScaler>().headTransform = head;
    }
}