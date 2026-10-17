using UnityEngine;
using System.Collections;

public class FanBlades : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("The maximum speed the fan will reach.")]
    public float maxRotationSpeed = 400f;

    [Tooltip("How fast the fan speeds up or slows down.")]
    public float acceleration = 150f;

    [Header("Axis Settings")]
    public Vector3 rotationAxis = Vector3.forward;

    [Header("Fan Audio")]
    public AudioSource audioSource;
    public AudioClip fanClickingSound;
    public AudioClip spinningSound;
    public AudioSource clickingAudioSource;

    [Header("Audio Fade Settings")]
    [Tooltip("How quickly the fan audio fades in and out.")]
    public float audioFadeSpeed = 0.5f;

    private bool isSpinning = false;
    private float currentSpeed = 0f;

    private float spinningOriginalVolume;
    private float clickingOriginalVolume;

    public bool IsSpinning => isSpinning;

    private Coroutine audioFadeCoroutine;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            spinningOriginalVolume = audioSource.volume;
            audioSource.Stop();
        }

        if (clickingAudioSource != null)
        {
            clickingAudioSource.playOnAwake = false;
            clickingAudioSource.loop = true;
            clickingOriginalVolume = clickingAudioSource.volume;
            clickingAudioSource.Stop();
        }
    }

    void Update()
    {
        float targetSpeed = isSpinning ? maxRotationSpeed : 0f;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.deltaTime
        );

        if (currentSpeed > 0f)
        {
            transform.Rotate(rotationAxis * currentSpeed * Time.deltaTime);
        }

        // Fade audio according to the fan's current rotation speed.
        float speedRatio = maxRotationSpeed > 0f
            ? Mathf.Clamp01(currentSpeed / maxRotationSpeed)
            : 0f;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.volume = spinningOriginalVolume * speedRatio;
        }

        if (clickingAudioSource != null &&
            clickingAudioSource.isPlaying)
        {
            clickingAudioSource.volume =
                clickingOriginalVolume * speedRatio;
        }

        // Stop the sounds only after the blades have stopped.
        if (!isSpinning && currentSpeed <= 0f)
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();

            if (clickingAudioSource != null &&
                clickingAudioSource.isPlaying)
            {
                clickingAudioSource.Stop();
            }
        }
    }

    public void ToggleFan()
    {
        isSpinning = !isSpinning;

        if (isSpinning)
        {
            if (audioSource != null && spinningSound != null)
            {
                audioSource.clip = spinningSound;
                audioSource.loop = true;
                audioSource.volume = 0f;

                if (!audioSource.isPlaying)
                    audioSource.Play();
            }

            if (clickingAudioSource != null &&
                fanClickingSound != null)
            {
                clickingAudioSource.clip = fanClickingSound;
                clickingAudioSource.loop = true;
                clickingAudioSource.volume = 0f;

                if (!clickingAudioSource.isPlaying)
                    clickingAudioSource.Play();
            }
        }
    }
}

