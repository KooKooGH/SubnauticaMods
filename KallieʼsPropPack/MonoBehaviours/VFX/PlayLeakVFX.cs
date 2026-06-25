using UnityEngine;

namespace KallieʼsPropPack.MonoBehaviours.VFX;

public class PlayLeakVFX : MonoBehaviour
{
    public VFXWaterSpray spray;
    public bool underwater;

    private void OnEnable()
    {
        spray.waterlevel = underwater ? 0 : Mathf.NegativeInfinity;
        if (!spray.GetIsPlaying())
            spray.Play();
    }
}