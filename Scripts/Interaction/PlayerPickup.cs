using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public Transform holdPoint;

    private Rigidbody heldObject;
    private GameObject heldBox;

    public Night1Manager night1Manager;

    [Header("Pickup Audio")]
    public AudioSource pickupAudioSource;
    public AudioClip boxPickupClip;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null)
                Pickup();
            else
                Drop();
        }
    }

    void FixedUpdate()
    {
        if (heldObject == null)
            return;

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 targetPosition = holdPoint.position;

        Vector3 direction = targetPosition - cameraPosition;
        float distance = direction.magnitude;

        if (Physics.Raycast(
            cameraPosition,
            direction.normalized,
            out RaycastHit hit,
            distance,
            ~LayerMask.GetMask("HeldObject")))
        {
            targetPosition = hit.point - direction.normalized * 0.3f;
        }

        heldObject.MovePosition(targetPosition);
        heldObject.MoveRotation(holdPoint.rotation);
    }

    void Pickup()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (!hit.collider.CompareTag("Pickup") &&
                hit.collider.GetComponentInParent<Rigidbody>() == null)
                return;

            Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();

            if (rb == null)
                return;

            GameObject box = rb.gameObject;

            if (!night1Manager.CanPickup(box))
            {
                Debug.Log("Cannot pick up " + box.name + " yet.");
                return;
            }

            heldObject = rb;
            heldBox = box;

            heldObject.isKinematic = true;
            heldObject.useGravity = false;

            // Tell Night1Manager that this box has left the ground
            night1Manager.BoxPickedUp(box);

            // Play box pickup sound
            if (pickupAudioSource != null && boxPickupClip != null)
            {
                pickupAudioSource.PlayOneShot(boxPickupClip);
            }
        }
    }

    void Drop()
    {
        heldObject.isKinematic = false;
        heldObject.useGravity = true;

        // The box remains active after being dropped
        night1Manager.BoxDropped(heldBox);

        heldObject = null;
        heldBox = null;
    }
}