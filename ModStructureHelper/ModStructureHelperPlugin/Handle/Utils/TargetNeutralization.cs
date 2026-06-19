using System.Collections.Generic;
using UnityEngine;

namespace ModStructureHelperPlugin.Handle.Utils;

// This is a pretty fun name for a class
public static class TargetNeutralization
{
    private static readonly Dictionary<Transform, bool> WasKinematic = new();
    private static readonly Dictionary<Transform, bool> WasPickupable = new();

    public static void NeutralizeTarget(Transform newTarget)
    {
        // Set and remember if rigidbody is kinematic
        var newRb = newTarget.GetComponent<Rigidbody>();
        bool kinematic = !newRb || newRb.isKinematic;
        WasKinematic[newTarget] = kinematic;
        
        // Set and remember if pickupable
        var newPickupable = newTarget.GetComponent<Pickupable>();
        bool isPickupable = newPickupable && newPickupable.isPickupable;
        WasPickupable[newTarget] = isPickupable;

        if (newRb)
        {
            newRb.isKinematic = true;
        }

        if (newPickupable)
        {
            newPickupable.isPickupable = false;
        }
    }
    
    public static void RestoreAll()
    {
        foreach (KeyValuePair<Transform, bool> kvp in WasKinematic)
        {
            if (kvp.Key == null)
                continue;
            
            var rigidbody = kvp.Key.GetComponent<Rigidbody>();
            if (rigidbody) rigidbody.isKinematic = kvp.Value;
        }

        foreach (KeyValuePair<Transform, bool> kvp in WasPickupable)
        {
            if (kvp.Key == null)
                continue;
            
            var pickupable = kvp.Key.GetComponent<Pickupable>();
            if (pickupable) pickupable.isPickupable = kvp.Value;
        }

        WasKinematic.Clear();
        WasPickupable.Clear();
    }
}