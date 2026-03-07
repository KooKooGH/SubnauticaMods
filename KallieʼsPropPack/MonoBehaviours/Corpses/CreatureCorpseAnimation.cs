using UnityEngine;

namespace KallieʼsPropPack.MonoBehaviours.Corpses;

public class CreatureCorpseAnimation : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        SetDeadAnimation();
    }

    private void OnEnable()
    {
        SetDeadAnimation();
    }

    private void SetDeadAnimation()
    {
        animator.speed = 35;
        animator.SetBool("dead", true);
    }
}