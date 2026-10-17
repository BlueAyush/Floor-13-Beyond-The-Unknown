using UnityEngine;

public class BoxDeliveryZone : MonoBehaviour
{
    public Night1Manager night1Manager;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponentInParent<Rigidbody>();

        if (rb == null)
            return;

        // Don't deliver while the player is holding the box
        if (rb.isKinematic)
            return;

        GameObject box = rb.gameObject;

        // Only deliver the currently active box
        if (!night1Manager.CanPickup(box))
            return;

        night1Manager.BoxDelivered(box);
    }
}