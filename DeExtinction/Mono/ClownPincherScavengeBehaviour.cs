﻿using UnityEngine;

namespace DeExtinction.Mono;

public class ClownPincherScavengeBehaviour : CreatureAction
{
    public ClownPincherCreature clownPincher;

    public float swimVelocity;
    private float _timeSearchAgain;
    private float _timeStarted;
    private GameObject _currentTarget;
    
    private EcoRegion.TargetFilter _targetFilter;
    
    private const float PriorityWhileScavenging = 0.8f;
    private const float FoodSearchInterval = 2f;

    private void Start()
    {
        _targetFilter = IsValidTarget;
    }

#if SUBNAUTICA
    public override float Evaluate(Creature creature, float time)
#elif BELOWZERO
    public override float Evaluate(float time)
#endif
    {
        if (clownPincher.lastAction == this && Time.time > _timeStarted + 20f)
        {
            return Time.time > _timeStarted + 20f ? 0f : PriorityWhileScavenging;
        }

        if (clownPincher.nibble.CurrentlyEating)
        {
            return PriorityWhileScavenging;
        }

        if (creature.Hunger.Value >= 0.9f)
        {
            if (TrySearchForFood(out IEcoTarget result))
            {
                return evaluatePriority;
            }
            clownPincher.nibble.EatWaterParticles();
        }

        return 0f;
    }

#if SUBNAUTICA
    public override void StartPerform(Creature creature, float time)
#elif BELOWZERO
    public override void StartPerform(float time)
#endif
    {
        _timeStarted = Time.time;
        if (TrySearchForFood(out IEcoTarget ecoTarget))
        {
            SetCurrentTarget(ecoTarget);
        }
    }

#if SUBNAUTICA
    public override void Perform(Creature creature, float time, float deltaTime)
#elif BELOWZERO
    public override void Perform(float time, float deltaTime)
#endif
    {
        if (Time.time > _timeSearchAgain)
        {
            _timeSearchAgain = Time.time + FoodSearchInterval;
            if (_currentTarget == null)
            {
                if (TrySearchForFood(out IEcoTarget result))
                {
                    SetCurrentTarget(result);
                }
            }
        }
        
        if (clownPincher.nibble.CurrentlyEating)
        {
            swimBehaviour.LookAt(clownPincher.nibble.currentlyEating.transform);
        }
        else if (_currentTarget != null)
        {
            swimBehaviour.SwimTo(_currentTarget.transform.position, swimVelocity);
        }
    }

    void SetCurrentTarget(IEcoTarget newTarget)
    {
        if (newTarget != null && newTarget.GetGameObject() != null)
        {
            _currentTarget = newTarget.GetGameObject();
            swimBehaviour.SwimTo(_currentTarget.transform.position, swimVelocity);
        }
        else
        {
            _currentTarget = null;
        }
    }

    private bool IsValidTarget(IEcoTarget target)
    {
        if (Random.value > 0.75f) return false;
        if (target == null || target.GetGameObject() == null) return false;
        return Vector3.Distance(transform.position, target.GetPosition()) < 35f;
    }

    bool TrySearchForFood(out IEcoTarget result)
    {
        result = null;
        var specialEdible =
            EcoRegionManager.main.FindNearestTarget(Plugin.ClownPincherFoodEcoTargetType, transform.position, _targetFilter,
                1);
        if (specialEdible != null && specialEdible.GetGameObject() != null)
        {
            result = specialEdible;
            return true;
        }

        var deadMeat =
            EcoRegionManager.main.FindNearestTarget(EcoTargetType.DeadMeat, transform.position, _targetFilter, 1);
        if (deadMeat != null && deadMeat.GetGameObject() != null)
        {
            result = deadMeat;
            return true;
        }

        return false;
    }
}