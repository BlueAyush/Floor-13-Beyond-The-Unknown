
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    [Header("Door References")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Door Settings")]
    public float openDistance = 0.75f;
    public float speed = 2f;

    [Header("Auto Close")]
    public float autoCloseDelay = 3f;

    public Vector3 leftDoorDirection = Vector3.back;
    public Vector3 rightDoorDirection = Vector3.forward;

    [Header("Elevator")]
    public ElevatorManager elevatorManager;

    [Header("Door Audio")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip doorCloseClip;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool isOpen = false;
    private float autoCloseTimer = 0f;

    void Start()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        leftOpenPos = leftClosedPos +
            leftDoorDirection.normalized * openDistance;

        rightOpenPos = rightClosedPos +
            rightDoorDirection.normalized * openDistance;
    }

    void Update()
    {
        Vector3 leftTarget = isOpen ? leftOpenPos : leftClosedPos;
        Vector3 rightTarget = isOpen ? rightOpenPos : rightClosedPos;

        leftDoor.localPosition = Vector3.MoveTowards(
            leftDoor.localPosition,
            leftTarget,
            speed * Time.deltaTime
        );

        rightDoor.localPosition = Vector3.MoveTowards(
            rightDoor.localPosition,
            rightTarget,
            speed * Time.deltaTime
        );

        if (isOpen)
        {
            autoCloseTimer -= Time.deltaTime;

            if (autoCloseTimer <= 0f)
                CloseDoors();
        }
    }

    public void OpenDoors()
    {
        if (elevatorManager != null && elevatorManager.IsMoving())
        {
            Debug.Log("Cannot open doors while elevator is moving!");
            return;
        }

        // Avoid replaying the sound if already open.
        if (isOpen)
        {
            autoCloseTimer = autoCloseDelay;
            return;
        }

        Debug.Log("OPEN DOORS");

        isOpen = true;
        autoCloseTimer = autoCloseDelay;

        if (doorAudioSource != null && doorOpenClip != null)
            doorAudioSource.PlayOneShot(doorOpenClip);
    }

    public void CloseDoors()
    {
        // Avoid replaying the sound if already closed.
        if (!isOpen)
            return;

        Debug.Log("CLOSE DOORS");

        isOpen = false;
        autoCloseTimer = 0f;

        if (doorAudioSource != null && doorCloseClip != null)
            doorAudioSource.PlayOneShot(doorCloseClip);
    }
}