using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsWaving", true); // 初始状态为挥手
    }

    public void StopWaving()
    {
        animator.SetBool("IsWaving", false); // 停止挥手
    }
}