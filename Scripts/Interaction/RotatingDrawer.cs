using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RotatingDrawer : MonoBehaviour, IInteractables
{
    [Header("Rotation Settings")]
    [Tooltip("The maximum angle (in degrees) the drawer swings open.")]
    public float openAngle = 90f;

    [Tooltip("How fast the drawer swings open and closed.")]
    public float rotationSpeed = 150f;

    [Tooltip("The local axis the drawer rotates around. Y = standard cabinet, X = tilt down, Z = roll.")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Drawer Audio")]
    public AudioSource asource;
    public AudioClip openDoor;
    public AudioClip closeDoor;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        closedRotation = transform.localRotation;

        if (asource == null)
            asource = GetComponent<AudioSource>();
    }
    void Update()
    {
        // 1. Calculate the goal rotation based on state
        Quaternion desiredRotation = isOpen
            ? closedRotation * Quaternion.AngleAxis(openAngle, rotationAxis)
            : closedRotation;

        // 2. Smoothly rotate the drawer toward that position every frame
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }

    // Triggers when the player looks at and interacts with the drawer
    public void Interact()
    {
        OpenDoor();
        Debug.Log($"Rotating drawer interacted! Is Open: {isOpen}");
    }

    public void OpenDoor()
    {
        isOpen = !isOpen;

        AudioClip clip = isOpen ? openDoor : closeDoor;

        if (asource != null && clip != null)
        {
            asource.PlayOneShot(clip);
        }
    }
}
