using UnityEngine;

namespace PodshellLeviathan.Mono;

public class DisableRedPlagueAggression : MonoBehaviour, IScheduledUpdateBehaviour
{
    private bool _completedMyPurpose;
    
    public string GetProfileTag()
    {
        return "Podshell:DisableRedPlagueAggression";
    }

    public void ScheduledUpdate()
    {
        if (_completedMyPurpose)
            return;
        
        var attack = GetComponent<AttackLastTarget>();
        if (attack != null)
        {
            attack.evaluatePriority = 0;
            _completedMyPurpose = true;
        }
        
        var melee = GetComponent<MeleeAttack>();
        if (melee != null)
        {
            melee.canBiteCreature = false;
            melee.canBitePlayer = false;
            melee.canBiteVehicle = false;
            melee.canBiteCyclops = false;
            melee.enabled = false;
        }
    }

    public int scheduledUpdateIndex { get; set; }

    private void OnEnable()
    {
        UpdateSchedulerUtils.Register(this);
    }

    private void OnDisable()
    {
        UpdateSchedulerUtils.Deregister(this);
    }
}