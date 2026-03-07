using UnityEngine;

namespace IceDragon.Content.FrozenIceDragon;

public class FrozenDragonFlail : MonoBehaviour, IManagedUpdateBehaviour
{
    public float interval = 8.733f;
    public Transform soundPosition;
    public FMODAsset sound;

    private float _timeNextPlay;
    
    private void OnEnable()
    {
        BehaviourUpdateUtils.Register(this);
    }

    private void OnDisable()
    {
        BehaviourUpdateUtils.Deregister(this);
    }

    public string GetProfileTag()
    {
        return "FrozenDragonFlail";
    }
    
    public void ManagedUpdate()
    {
        if (Time.time < _timeNextPlay)
            return;
        FMODUWE.PlayOneShot(sound, soundPosition.position);
        _timeNextPlay = Time.time + interval;
    }

    public int managedUpdateIndex { get; set; }
}