
using System.Collections;
using UnityEngine;
using StarterAssets;

public class Night2WakeUp : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform wakeUpPoint;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private FirstPersonController playerController;

    [Header("Wake-Up Dialogue")]
    [SerializeField] private float startDelay = 0.5f;

    [TextArea]
    [SerializeField]
    private string wakeUpLine =
        "Alright, let's get ready for work. Don't want to be late.";

    private IEnumerator Start()
    {
        // Let the scene and FloorManager initialize first.
        yield return null;

        if (NightManager.Instance == null ||
            NightManager.Instance.CurrentNight != 2)
        {
            yield break;
        }

        // Wait briefly before starting the wake-up sequence.
        yield return new WaitForSeconds(startDelay);

        // Place Arjun at the chosen starting position.
        if (player != null && wakeUpPoint != null)
        {
            if (characterController != null)
                characterController.enabled = false;

            player.SetPositionAndRotation(
                wakeUpPoint.position,
                wakeUpPoint.rotation
            );

            if (characterController != null)
                characterController.enabled = true;
        }

        // Prevent movement while Arjun's thought is displayed.
        // if (playerController != null)
        // {
        //     playerController.LockMovement();
        // }

        if (DialogueManager.Instance != null)
        {
            yield return StartCoroutine(
                DialogueManager.Instance.PlaySleepMonologue(
                    wakeUpLine
                )
            );
        }

        // Restore normal player controls.
        // if (playerController != null)
        // {
        //     playerController.UnlockMovement();
        // }
    }
}
