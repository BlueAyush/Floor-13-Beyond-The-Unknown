
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private FirstPersonController playerController;

    [Tooltip("Child AudioSource named FootstepAudio.")]
    [SerializeField] private AudioSource outdoorAudioSource;

    [Tooltip("Child AudioSource named IndoorFootstepAudio.")]
    [SerializeField] private AudioSource indoorAudioSource;

    [Header("Outdoor Footsteps")]
    [SerializeField] private AudioClip[] outsideFootstepClips;

    [Header("Indoor Footsteps")]
    [SerializeField] private AudioClip[] insideFootstepClips;

    [Header("Environment Detection")]
    [Tooltip("Layers that should use indoor footsteps.")]
    [SerializeField] private LayerMask indoorLayers;

    [Tooltip("Layers that can be detected beneath the player.")]
    [SerializeField] private LayerMask groundLayers = ~0;

    [Tooltip("Maximum distance for detecting the floor.")]
    [SerializeField] private float groundCheckDistance = 1.5f;

    [Header("Footstep Timing")]
    [SerializeField] private float walkStepInterval = 0.48f;
    [SerializeField] private float sprintStepInterval = 0.32f;
    [SerializeField] private float minimumMoveSpeed = 0.15f;


    private float stepTimer;
    private float stopGraceTimer;
    private bool wasMoving;
    private bool isIndoors;
    private int lastClipIndex = -1;

    private AudioClip[] currentClips;
    private AudioSource currentAudioSource;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (playerController == null)
            playerController = GetComponent<FirstPersonController>();

        if (outdoorAudioSource == null)
        {
            Transform child = transform.Find("FootstepAudio");

            if (child != null)
                outdoorAudioSource = child.GetComponent<AudioSource>();
        }

        if (indoorAudioSource == null)
        {
            Transform child = transform.Find("IndoorFootstepAudio");

            if (child != null)
                indoorAudioSource = child.GetComponent<AudioSource>();
        }

        UpdateFootstepSet(false);
    }

    private void Update()
    {
        if (characterController == null ||
            playerController == null ||
            outdoorAudioSource == null ||
            indoorAudioSource == null)
        {
            return;
        }

        if (!playerController.Grounded)
        {
            stepTimer = 0f;
            stopGraceTimer = 0f;
            wasMoving = false;
            return;
        }

        // Detect which layer is directly beneath the player.
        bool detectedIndoors = CheckIfIndoors();

        if (detectedIndoors != isIndoors)
            UpdateFootstepSet(detectedIndoors);

        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;

        float horizontalSpeed = velocity.magnitude;
        bool isMoving = horizontalSpeed > minimumMoveSpeed;

        if (!isMoving)
        {
            stopGraceTimer += Time.deltaTime;

            if (stopGraceTimer > 0.2f)
            {
                stepTimer = 0f;
                wasMoving = false;
            }

            return;
        }

        stopGraceTimer = 0f;

        bool isSprinting =
            horizontalSpeed > playerController.MoveSpeed + 0.5f;

        float interval = isSprinting
            ? sprintStepInterval
            : walkStepInterval;

        if (!wasMoving)
        {
            PlayFootstep();
            stepTimer = 0f;
            wasMoving = true;
            return;
        }

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private bool CheckIfIndoors()
    {
        Vector3 origin = characterController.bounds.center;
        origin.y = characterController.bounds.min.y + 0.1f;

        if (Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayers,
            QueryTriggerInteraction.Ignore))
        {
            int hitLayer = 1 << hit.collider.gameObject.layer;
            return (indoorLayers.value & hitLayer) != 0;
        }

        // Keep the previous setting if no floor is detected.
        return isIndoors;
    }

    private void UpdateFootstepSet(bool indoors)
    {
        isIndoors = indoors;

        if (isIndoors)
        {
            currentClips = insideFootstepClips;
            currentAudioSource = indoorAudioSource;

        }
        else
        {
            currentClips = outsideFootstepClips;
            currentAudioSource = outdoorAudioSource;

        }

        lastClipIndex = -1;
    }

    private void PlayFootstep()
    {
        if (currentClips == null ||
            currentClips.Length == 0 ||
            currentAudioSource == null)
        {
            return;
        }

        // Try to select a valid clip without repeating the previous one.
        int validClipCount = 0;

        for (int i = 0; i < currentClips.Length; i++)
        {
            if (currentClips[i] != null)
                validClipCount++;
        }

        if (validClipCount == 0)
            return;

        int index;
        int attempts = 0;

        do
        {
            index = Random.Range(0, currentClips.Length);
            attempts++;
        }
        while (
            (currentClips[index] == null ||
             (currentClips.Length > 1 && index == lastClipIndex)) &&
            attempts < 20
        );

        // Fallback if random selection repeatedly finds an empty slot.
        if (currentClips[index] == null ||
            (currentClips.Length > 1 && index == lastClipIndex))
        {
            index = -1;

            for (int i = 0; i < currentClips.Length; i++)
            {
                if (currentClips[i] != null && i != lastClipIndex)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                for (int i = 0; i < currentClips.Length; i++)
                {
                    if (currentClips[i] != null)
                    {
                        index = i;
                        break;
                    }
                }
            }
        }

        if (index == -1)
            return;

        lastClipIndex = index;
        currentAudioSource.PlayOneShot(currentClips[index]);
    }

}