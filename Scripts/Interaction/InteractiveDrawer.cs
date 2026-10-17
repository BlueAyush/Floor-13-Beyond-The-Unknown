using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractiveDrawer : MonoBehaviour, IInteractables
{
[Header("Movement Settings")]
[Tooltip("How far the drawer slides open (in Unity meters).")]
public float openDistance = 0.5f;


[Tooltip("How fast the drawer slides open and closed.")]
public float slideSpeed = 2f;

[Tooltip("The local direction the drawer opens. Z is forward, X is right.")]
public Vector3 openDirection = Vector3.forward;

[Header("Drawer Audio")]
public AudioSource asource;
public AudioClip openDrawer;
public AudioClip closeDrawer;

private bool isOpen = false;
private Vector3 closedPosition;

void Start()
{
    // Save the starting position as the closed position
    closedPosition = transform.localPosition;

    if (asource == null)
        asource = GetComponent<AudioSource>();
}

void Update()
{
    Vector3 targetPosition = isOpen
        ? closedPosition + (openDirection.normalized * openDistance)
        : closedPosition;

    transform.localPosition = Vector3.MoveTowards(
        transform.localPosition,
        targetPosition,
        slideSpeed * Time.deltaTime
    );
}

public void Interact()
{
    OpenDrawer();
}

public void OpenDrawer()
{
    isOpen = !isOpen;

    AudioClip clip = isOpen ? openDrawer : closeDrawer;

    if (asource != null && clip != null)
    {
        asource.PlayOneShot(clip);
    }

    Debug.Log($"Drawer interacted! Is Open: {isOpen}");
}


}
