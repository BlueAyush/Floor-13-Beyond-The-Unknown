using System.Collections;
using UnityEngine;

public class ElevatorTravel : MonoBehaviour
{
    public ElevatorDoor doorController;
    public FloorDisplay floorDisplay;
    public HallwayManager hallwayManager;

    public GameObject[] hallways;
    public Transform player;

    public float travelTime;

    bool isTravelling = false;

    // Pass the target floor number into this method (e.g., from a button click)
    public void StartTravel(int targetFloor)
    {
        if (!isTravelling)
        {
            StartCoroutine(TravelFloors(targetFloor));
        }
    }

    IEnumerator TravelFloors(int targetFloor)
    {
        isTravelling = true;

        doorController.CloseDoors();


        // Wait for the elevator to travel
        yield return new WaitForSeconds(travelTime);

        // Update the floor number BEFORE changing the display or hallway
        HallwayManager.Instance.currentFloor = targetFloor;

        // Update the UI display
        floorDisplay.SetFloor(HallwayManager.Instance.currentFloor);

        yield return new WaitForSeconds(0.15f);

        // Switch the physical game objects
        HallwayManager.Instance.SwitchFloors();

        yield return new WaitForSeconds(0.5f);


        doorController.OpenDoors();

        isTravelling = false;
    }
}
