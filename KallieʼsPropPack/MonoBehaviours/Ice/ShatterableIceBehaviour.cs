using System.Collections.Generic;
using UnityEngine;

namespace KallieʼsPropPack.MonoBehaviours.Ice;

public class ShatterableIceBehaviour : MonoBehaviour
{
    private static readonly List<ShatterableIceBehaviour> Instances = new();

    public GameObject normalVersion;
    public GameObject shatteredVersion;

    public FMODAsset shatterSound;

    private bool _isShattered;

    private void Awake()
    {
        if (normalVersion != null) normalVersion.SetActive(true);
        if (shatteredVersion != null) shatteredVersion.SetActive(false);
        _isShattered = false;
    }

    private void OnEnable()
    {
        if (!Instances.Contains(this))
            Instances.Add(this);
    }

    private void OnDisable()
    {
        Instances.Remove(this);
    }

    public void Shatter()
    {
        if (_isShattered)
            return;

        _isShattered = true;

        if (normalVersion != null)
            normalVersion.SetActive(false);

        if (shatteredVersion != null)
            shatteredVersion.SetActive(true);

        if (shatterSound != null)
            FMODUWE.PlayOneShot(shatterSound, transform.position);
    }
    
    public static void ShatterInRadius(Vector3 point, float radius)
    {
        float radiusSqr = radius * radius;

        for (int i = 0; i < Instances.Count; i++)
        {
            ShatterableIceBehaviour ice = Instances[i];
            if (ice == null || ice._isShattered)
                continue;

            Vector3 delta = ice.transform.position - point;
            if (delta.sqrMagnitude <= radiusSqr)
            {
                ice.Shatter();
            }
        }
    }
}