using System.Collections.Generic;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using UnityEngine;

namespace PodshellLeviathan.Mono.Baby;

public class BabyPodshellCommands : HandTarget, IHandTarget
{
    public WaterParkCreature waterPark;
    public Animator animator;
    public PrefabIdentifier identifier;
    public PodshellVoice voice;
    public CreatureFollowPlayer follow;
    public float commandInterval = 0.2f;
    
    private State _state;

    private static SaveData _saveData;

    private float _timeCommandAgain;

    public State GetCurrentState() => _state;

    private void Start()
    {
        if (!TryLoadSaveData())
        {
            SetState(State.Wandering);
        }
    }

    private bool TryLoadSaveData()
    {
        if (_saveData == null)
        {
            Plugin.Logger.LogWarning("Podshell baby commands SaveData not found!");
            return false;
        }

        if (_saveData.savedStates == null)
        {
            return false;
        }

        if (_saveData.savedStates.TryGetValue(identifier.Id, out var state))
        {
            SetState(state);
            return true;
        }
        
        return false;
    }

    public void OnHandHover(GUIHand hand)
    {
        if (waterPark.IsInsideWaterPark())
            return;

        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, GetCommandText(GetNextState()), true, GameInput.Button.LeftHand); 
        HandReticle.main.SetText(HandReticle.TextType.Hand, GetCurrentStateText(_state), true); 
    }

    public void OnHandClick(GUIHand hand)
    {
        if (waterPark.IsInsideWaterPark())
            return;
        
        if (Time.time < _timeCommandAgain)
            return;

        _timeCommandAgain = Time.time + commandInterval;
        voice.PlayRoarSound(false);
        animator.SetTrigger("small_roar");
        
        SetState(GetNextState());
    }

    private void SetState(State state)
    {
        _state = state;
        _saveData.savedStates ??= new Dictionary<string, State>();
        _saveData.savedStates[identifier.Id] = state;
        follow.enabled = state == State.Following;
    }

    private State GetNextState()
    {
        switch (_state)
        {
            case State.Wandering:
                return State.Following;
            case State.Following:
                return State.Idle;
            case State.Idle:
                return State.Wandering;
            default:
                Plugin.Logger.LogWarning("Unrecognized state for Podshell baby: " + _state);
                return State.Wandering;
        }
    }
    
    private string GetCommandText(State state)
    {
        return $"PodshellBabyCommand_{state}";
    }
    
    private string GetCurrentStateText(State state)
    {
        return $"PodshellBabyState_{state}";
    }


    public static void RegisterSaveData()
    {
        _saveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
    }

    public enum State
    {
        Wandering,
        Following,
        Idle
    }

    [FileName("PodshellBabyCommands")]
    private class SaveData : SaveDataCache
    {
        public Dictionary<string, State> savedStates;
    }
}