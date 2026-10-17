using UnityEngine;

public class DialogueAnimationController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Animation Parameters")]
    public string talkingParameter = "IsTalking";

    public void StartTalking()
    {
        if (animator == null)
            return;

        animator.SetBool(talkingParameter, true);
    }

    public void StopTalking()
    {
        if (animator == null)
            return;

        animator.SetBool(talkingParameter, false);
    }
}