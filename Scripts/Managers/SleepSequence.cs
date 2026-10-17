
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class SleepSequence : MonoBehaviour
{
    public static SleepSequence Instance;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private FirstPersonController playerController;

    [Header("Camera")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform sleepCameraPoint;

    [Header("Sleep Position")]
    [SerializeField] private Transform sleepPoint;

    [Header("Fan")]
    [SerializeField] private FanBlades fanBlades;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Day / Night Transition")]
    [SerializeField] private TMP_Text transitionText;
    [SerializeField] private string transitionMessage = "DAY 2";
    [SerializeField] private float textDisplayDuration = 2f;

    [Header("Transition Sound")]
    [SerializeField] private bool playTransitionSound = false;
    [SerializeField] private AudioSource transitionAudioSource;
    [SerializeField] private AudioClip transitionSound;

    [Header("Sleep Settings")]
    [SerializeField] private float lieDownDuration = 1f;

    private void Awake()
    {
        Instance = this;

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }

        if (transitionText != null)
        {
            transitionText.gameObject.SetActive(false);
        }
    }

    public void StartSleep()
    {
        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        // Lock player controls
        if (playerController != null)
        {
            playerController.LockMovement();
            playerController.LockLook();
        }

        // Move player to the bed
        if (player != null && sleepPoint != null)
        {
            player.position = sleepPoint.position;
            player.rotation = sleepPoint.rotation;
        }

        // Move camera to lying-down position
        if (mainCamera != null && sleepCameraPoint != null)
        {
            mainCamera.position = sleepCameraPoint.position;
            mainCamera.rotation = sleepCameraPoint.rotation;
        }

        // Allow the player to settle into bed
        yield return new WaitForSeconds(lieDownDuration);

        // Fan OFF -> play monologue
        if (fanBlades != null && !fanBlades.IsSpinning)
        {
            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(
                    DialogueManager.Instance.PlaySleepMonologue(
                        "Should have turned on the fan, but anyways."
                    )
                );
            }
        }

        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Show manually configured transition text
        yield return StartCoroutine(
            ShowTransition(transitionMessage)
        );

        Debug.Log("Player is asleep.");

        // Load the next night
        if (NightManager.Instance != null)
        {
            NightManager.Instance.LoadNextNight();
        }
    }

    public IEnumerator FadeToBlack()
    {
        if (fadeImage == null)
            yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;
        float startingAlpha = color.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            color.a = Mathf.Lerp(
                startingAlpha,
                1f,
                elapsed / fadeDuration
            );

            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    public IEnumerator ShowTransition(string message)
    {
        // Show the text over the black screen
        if (transitionText != null)
        {
            transitionText.text = message;
            transitionText.gameObject.SetActive(true);
        }

        // Play sound only when enabled and assigned
        if (playTransitionSound &&
            transitionAudioSource != null &&
            transitionSound != null)
        {
            transitionAudioSource.PlayOneShot(transitionSound);
        }

        yield return new WaitForSeconds(textDisplayDuration);

        if (transitionText != null)
        {
            transitionText.gameObject.SetActive(false);
        }
    }
}
