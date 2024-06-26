﻿using System;
using Nautilus.Extensions;
using Nautilus.Utility;
using UnityEngine;

namespace DeExtinction.Mono;

public class GulperMeleeAttackMouth : MeleeAttack
{
    public bool isBaby; // implies GulperBehaviour DNE
    private FMODAsset _genericAttackSound = AudioUtils.GetFmodAsset("GulperBite");
    private FMODAsset _playerAttackSound = AudioUtils.GetFmodAsset("GulperBitePlayer");
    private PlayerCinematicController _playerDeathCinematic;
    private Transform _throat;
    private float _timeCanPlayCinematicAgain;
    private FMOD_CustomEmitter _emitter;
    private static readonly int Bite = Animator.StringToHash("bite");

    private void Start()
    {
        _playerDeathCinematic = gameObject.AddComponent<PlayerCinematicController>();
        _playerDeathCinematic.animatedTransform = gameObject.SearchChild("PlayerCam").transform;
        _playerDeathCinematic.animator = creature.GetAnimator();
        _playerDeathCinematic.animParamReceivers = Array.Empty<GameObject>();
#if SUBNAUTICA
        _playerDeathCinematic.playerViewAnimationName = "reaper_attack";
#elif BELOWZERO
        _playerDeathCinematic.playerViewAnimationName = "shadowLevi_player_attack";
#endif
        _throat = gameObject.SearchChild("Throat").transform;
        _emitter = gameObject.AddComponent<FMOD_CustomEmitter>();
        _emitter.followParent = true;
    }

    public override void OnTouch(Collider collider)
    {
        if (frozen || !isActiveAndEnabled)
        {
            return;
        }

        if (liveMixin.IsAlive() && Time.time > timeLastBite + biteInterval)
        {
            Creature component = GetComponent<Creature>();

            GameObject target = GetTarget(collider);
            GulperBehaviour gulperBehaviour = GetComponent<GulperBehaviour>();
            if ((isBaby || !gulperBehaviour.IsHoldingVehicle()) && !_playerDeathCinematic.IsCinematicModeActive())
            {
                Player player = target.GetComponent<Player>();
                if (player != null && !isBaby)
                {
                    if (Time.time > _timeCanPlayCinematicAgain && player.CanBeAttacked() && player.liveMixin.IsAlive() &&
                        !player.cinematicModeActive)
                    {
                        Invoke("KillPlayer", 0.9f);
                        _playerDeathCinematic.StartCinematicMode(player);
                        _emitter.SetAsset(_playerAttackSound);
                        _emitter.Play();
                        _timeCanPlayCinematicAgain = Time.time + 4f;
                        timeLastBite = Time.time;
                        return;
                    }
                }
                else if (!isBaby && gulperBehaviour.GetCanGrabVehicle() && component.Aggression.Value > 0.1f)
                {
                    Exosuit exosuit = target.GetComponent<Exosuit>();
                    if (exosuit && !exosuit.docked)
                    {
                        gulperBehaviour.GrabExosuit(exosuit);
                        component.Aggression.Value -= 0.5f;
                        return;
                    }
                    
                    Vehicle genericVehicle = target.GetComponent<Vehicle>();
                    if (genericVehicle && !genericVehicle.docked)
                    {
                        gulperBehaviour.GrabGenericSub(genericVehicle);
                        component.Aggression.Value -= 0.5f;
                        return;
                    }
                }

                LiveMixin liveMixin = target.GetComponent<LiveMixin>();
                if (liveMixin == null) return;
                if (isBaby && liveMixin.maxHealth > 90f && UnityEngine.Random.value > 0.1f)
                {
                    return;
                }

                if (!liveMixin.IsAlive())
                {
                    return;
                }

                if (!CanAttackTargetFromPosition(target))
                {
                    return;
                }

                if (CanSwallowWhole(collider.gameObject, liveMixin))
                {
                    Destroy(liveMixin.gameObject, 0.5f);
                    var suckInWhole = collider.gameObject.AddComponent<CreatureSwallowedAnimation>();
                    suckInWhole.animationLength = 0.5f;
                    suckInWhole.target = _throat;
                    _emitter.SetAsset(_genericAttackSound);
                    _emitter.Play();
                    creature.GetAnimator().SetTrigger(Bite);
                    return;
                }

                if (component.Aggression.Value >= 0.2f && target.GetComponentInParent<Vehicle>() == null)
                {
                    liveMixin.TakeDamage(GetBiteDamage(target));
                    timeLastBite = Time.time;
                    _emitter.SetAsset(_genericAttackSound);
                    _emitter.Play();
                    creature.GetAnimator().SetTrigger(Bite);
                    component.Aggression.Value = 0f;
                }
            }
        }
    }

    private bool CanAttackTargetFromPosition(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        float magnitude = direction.magnitude;
        int num = UWE.Utils.RaycastIntoSharedBuffer(transform.position, direction, magnitude, -5,
            QueryTriggerInteraction.Ignore);
        for (int i = 0; i < num; i++)
        {
            Collider collider = UWE.Utils.sharedHitBuffer[i].collider;
            GameObject gameObject = (collider.attachedRigidbody != null)
                ? collider.attachedRigidbody.gameObject
                : collider.gameObject;
            if (!(gameObject == target) && !(gameObject == base.gameObject) &&
                !(gameObject.GetComponent<Creature>() != null))
            {
                return false;
            }
        }

        return true;
    }

    private float GetBiteDamage(GameObject target)
    {
        if (target.GetComponent<SubControl>() != null)
        {
            return 50f; //cyclops damage
        }

        return biteDamage; //base damage
    }

    private bool CanSwallowWhole(GameObject gameObject, LiveMixin liveMixin)
    {
        if (gameObject.GetComponentInParent<Player>())
        {
            return false;
        }

        if (gameObject.GetComponentInChildren<Player>())
        {
            return false;
        }

        if (gameObject.GetComponentInParent<Vehicle>())
        {
            return false;
        }

        if (gameObject.GetComponentInParent<SubRoot>())
        {
            return false;
        }
        
#if BELOWZERO
        if (gameObject.GetComponentInParent<IInteriorSpace>() != null)
        {
            return false;
        }        
#endif

#if SUBNAUTICA
        if (gameObject.GetComponentInParent<EscapePod>())
        {
            return false;
        }
#endif

        if (liveMixin.maxHealth > 600f)
        {
            return false;
        }

        if (liveMixin.invincible)
        {
            return false;
        }

        return true;
    }

    private void KillPlayer()
    {
        Player.main.liveMixin.Kill(DamageType.Normal);
        _playerDeathCinematic.OnPlayerCinematicModeEnd();
    }

    public void OnVehicleReleased()
    {
        timeLastBite = Time.time;
    }
}