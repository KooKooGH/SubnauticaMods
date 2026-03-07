using UnityEngine;

namespace PodshellLeviathan.Mono.Baby;

public class BabyPodshellStay : CreatureAction, IManagedFixedUpdateBehaviour
{
    public BabyPodshellCommands commands;
    public Rigidbody rb;
    
    private const float VelocityDamp = 6f;
    
    public int managedFixedUpdateIndex { get; set; }
    
    public override float Evaluate(Creature creature, float time)
    {
        if (commands.GetCurrentState() == BabyPodshellCommands.State.Idle)
            return base.Evaluate(creature, time);
        
        return 0;
    }

    public override void StartPerform(Creature creature, float time)
    {
        swimBehaviour.Idle();
        BehaviourUpdateUtils.Register(this);
    }

    public override void StopPerform(Creature creature, float time)
    {
        base.StopPerform(creature, time);
        BehaviourUpdateUtils.Deregister(this);
    }

    private void OnDestroy()
    {
        BehaviourUpdateUtils.Deregister(this);
    }

    public void ManagedFixedUpdate()
    {
        if (!rb)
            return;

        var velocity = rb.velocity;

        if (velocity.sqrMagnitude < 0.0001f)
            return;

        rb.velocity = Vector3.Lerp(velocity, Vector3.zero, VelocityDamp * Time.fixedDeltaTime);
    }
    
    public string GetProfileTag()
    {
        return "Podshell:BabyPodshellStay";
    }
}