﻿using UnityEngine;

namespace DeExtinction.Mono;

public abstract class ReactToPredatorAction : CreatureAction
{
#if BELOWZERO
    public override void Start()
    {
        base.Start();
#endif
#if SUBNAUTICA
    private void Start()
    {
#endif
        Fear = gameObject.GetComponent<CreatureFear>();
    }

    public float maxReactDistance;
    public EcoTargetType fearedTargetType = EcoTargetType.Shark;
    public float actionLength = 1f;

    protected CreatureFear Fear;
    protected bool PerformingAction;
    protected float TimeStopAction;

    private bool _frozen;

#if SUBNAUTICA
    public override float Evaluate(Creature creature, float time)
#elif BELOWZERO
    public override float Evaluate(float time)
#endif
    {
        if (_frozen)
        {
            return 0f;
        }

        if (!CanPerform())
        {
            return 0f;
        }
        
        IEcoTarget closestTarget = EcoRegionManager.main.FindNearestTarget(fearedTargetType, transform.position, null, 1);
        if (closestTarget != null && closestTarget.GetGameObject() != null)
        {
            if (Vector3.Distance(closestTarget.GetPosition(), transform.position) < maxReactDistance)
            {
                PerformingAction = true;
                TimeStopAction = Time.time + actionLength;
                if (Fear) Fear.SetScarePosition(closestTarget.GetPosition());
            }
        }
        
        if (PerformingAction)
        {
            if (Time.time > TimeStopAction)
            {
                PerformingAction = false;
            }
        }

        return PerformingAction ? evaluatePriority : 0f;
    }

    protected virtual bool CanPerform() => true;

    public void OnFreeze()
    {
        _frozen = true;
    }

    public void OnUnfreeze()
    {
        _frozen = false;
    }
}