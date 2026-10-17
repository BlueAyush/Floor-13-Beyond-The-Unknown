using UnityEngine;

public class ElevatorDoorButton : MonoBehaviour, IInteractables
{
    public ElevatorDoor doorController;

    public void Interact()
    {
        doorController.OpenDoors();
    }
}