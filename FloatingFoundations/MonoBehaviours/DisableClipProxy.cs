using UnityEngine;

namespace FloatingFoundations.MonoBehaviours;

public class DisableClipProxy : MonoBehaviour, IScheduledUpdateBehaviour
{
    public GameObject clipProxy;
    public float maxDistanceSqr = 50 * 50;
    
    private void OnEnable()
    {
        UpdateSchedulerUtils.Register(this);
    }

    private void OnDisable()
    {
        UpdateSchedulerUtils.Deregister(this);
    }

    public string GetProfileTag()
    {
        return "FloatingFoundations:DisableClipProxy";
    }

    public void ScheduledUpdate()
    {
        clipProxy.SetActive(Vector3.SqrMagnitude(MainCameraControl.main.transform.position - transform.position) < maxDistanceSqr);
    }

    public int scheduledUpdateIndex { get; set; }
}