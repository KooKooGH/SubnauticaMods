using UnityEngine;

namespace PodshellLeviathan.Mono.Baby;

public class BabyHitGlassSound : MonoBehaviour
{
    public FMODAsset sound;
    public WaterParkCreature waterPark;
    public Rigidbody rb;

    public float minImpulseSqrMagnitude = 0.1f;
    public float maxDistanceSqr = 14 * 14;
    public float minInterval = 2f;
    
    public float minHeadOnDot = 0.4f;

    private float _timeCanPlayAgain;
    
    private void OnCollisionEnter(Collision other)
    {
        if (Time.time < _timeCanPlayAgain)
            return;

        if (!waterPark.IsInsideWaterPark())
            return;

        if (other.impulse.sqrMagnitude < minImpulseSqrMagnitude)
            return;
        
        var distanceToCamera = Vector3.SqrMagnitude(transform.position - waterPark.transform.position);
        if (distanceToCamera > maxDistanceSqr)
            return;
        
        // Check if the hit is head-on
        var contact = other.GetContact(0);
        float alignment = Vector3.Dot(rb.velocity.normalized, -contact.normal);
        
        if (alignment < minHeadOnDot)
            return;

        var go = other.gameObject;
        if (go.GetComponent<LiveMixin>() != null)
            return;

        _timeCanPlayAgain = Time.time + minInterval;
        FMODUWE.PlayOneShot(sound, transform.position);
    }
}