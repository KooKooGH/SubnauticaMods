using System.Collections;
using Story;
using UnityEngine;

namespace IceDragon.Content.FrozenIceDragon;

public class FrozenIceDragonRoarOnScan : MonoBehaviour
{
    public Animator animator;
    public FMODAsset sound;
    public Transform roarPosition;

    public float delay = 3f;
    
    // DO NOT CHANGE - PATCHED BY PLAGUE GARG MOD
    public void Start()
    {
        Plugin.Logger.LogMessage("FrozenIceDragonRoarOnScan start");
    }
    
    private void OnEnable()
    {
        PDAScanner.onAdd += OnAdd;
    }

    private void OnDisable()
    {
        PDAScanner.onAdd -= OnAdd;
    }

    private void OnAdd(PDAScanner.Entry entry)
    {
        var story = StoryGoalManager.main;
        if (!story)
        {
            Plugin.Logger.LogError("Story goal manager does not exist!");
            return;
        }

        if (story.OnGoalComplete(ModStory.FrozenIceDragonRoar.key))
        {
            StartCoroutine(RoarDelayed());
        }
    }

    private IEnumerator RoarDelayed()
    {
        yield return new WaitForSeconds(delay);
        
        FMODUWE.PlayOneShot(sound, roarPosition.position);
        animator.SetTrigger("roar");

        var camera = MainCameraControl.main;
        camera.ShakeCamera(0.7f, 3f, MainCameraControl.ShakeMode.BuildUp, 0.9f);
        yield return new WaitForSeconds(3);
        camera.ShakeCamera(2f, 1f, MainCameraControl.ShakeMode.Quadratic, 1.2f);
        yield return new WaitForSeconds(1);
        camera.ShakeCamera(3f, 5f, MainCameraControl.ShakeMode.Sqrt);
    }
}