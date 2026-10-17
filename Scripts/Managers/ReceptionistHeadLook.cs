using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ReceptionistHeadLook : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Rig headRig;

    [Header("Settings")]
    public float lookDistance = 4f;
    public float rigSpeed = 3f;

    void Update()
    {
        if (player == null || headRig == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        float targetWeight = distance <= lookDistance ? 1f : 0f;

        headRig.weight = Mathf.MoveTowards(
            headRig.weight,
            targetWeight,
            rigSpeed * Time.deltaTime
        );
    }
}