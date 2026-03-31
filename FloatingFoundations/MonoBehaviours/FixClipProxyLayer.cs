using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace FloatingFoundations.MonoBehaviours;

public class FixClipProxyLayer : MonoBehaviour, IConstructable
{
    public GameObject clipProxy;
        
    private void Start()
    {
        FixLayer();
    }

    private void FixLayer()
    {
        clipProxy.layer = LayerID.BaseClipProxy;
    }

    public bool IsDeconstructionObstacle()
    {
        return false;
    }

    public bool CanDeconstruct([UnscopedRef] out string reason)
    {
        reason = null;
        return true;
    }

    public void OnConstructedChanged(bool constructed)
    {
        FixLayer();
        StartCoroutine(FixLayerNextFrame());
    }

    private IEnumerator FixLayerNextFrame()
    {
        yield return null;
        FixLayer();
    }
}