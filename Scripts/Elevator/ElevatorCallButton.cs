using UnityEngine;

public class ElevatorCallButton : MonoBehaviour, IInteractables
{
    public ElevatorManager elevatorManager;

    public void Interact()
    {
        elevatorManager.CallElevator();
    }
}