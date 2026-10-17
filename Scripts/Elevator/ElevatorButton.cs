using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractables
{
    public ElevatorManager elevatorManager;
    public int floorNumber;

    [Header("Button Light")]
    public ElevatorButtonLight buttonLight;

    public void Interact()
    {
        elevatorManager.GoToFloor(floorNumber, buttonLight);
    }
}