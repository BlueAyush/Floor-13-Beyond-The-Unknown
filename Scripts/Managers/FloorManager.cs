
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [Header("0 = Ground Floor, 1 = Floor 1 ... 13 = Floor 13")]
    public GameObject[] floors;

    [Header("Night 1 Boxes")]
    public GameObject[] night1Boxes;

    [Header("Night 1 Manager")]
    public Night1Manager night1Manager;

    [Header("Starting Floor By Night")]
    [Tooltip("Array index = night number. Index 0 is unused.")]
    public int[] startingFloorsByNight = new int[]
    {
        0,  // Index 0 unused
        0,  // Night 1: Ground Floor
        4,  // Night 2: Floor 4 / Apartment 401
        0,  // Night 3: Ground Floor (change later if needed)
        0,  // Night 4
        0,  // Night 5
        0,  // Night 6
        0   // Night 7
    };

    public int CurrentFloor { get; private set; } = 0;

    void Start()
    {
        int startingFloor = 0;

        if (NightManager.Instance != null)
        {
            int night = NightManager.Instance.CurrentNight;

            if (night >= 1 && night < startingFloorsByNight.Length)
            {
                startingFloor = startingFloorsByNight[night];
            }
        }

        SetFloor(startingFloor);
    }

    public void SetFloor(int floorIndex)
    {

        if (floors == null ||
            floorIndex < 0 ||
            floorIndex >= floors.Length)
        {
            Debug.LogWarning("Invalid floor index: " + floorIndex);
            return;
        }

        for (int i = 0; i < floors.Length; i++)
        {
            if (floors[i] != null)
                floors[i].SetActive(i == floorIndex);
        }

        CurrentFloor = floorIndex;

        if (night1Manager != null)
            UpdateBoxVisibility();

        Debug.Log("Current Floor: " + CurrentFloor);
    }

    void UpdateBoxVisibility()
    {
        bool onGroundFloor = CurrentFloor == 0;

        if (night1Boxes == null)
            return;

        for (int i = 0; i < night1Boxes.Length; i++)
        {
            if (night1Boxes[i] == null)
                continue;

            bool pickedUp = night1Manager.WasBoxPickedUp(i);
            bool delivered = night1Manager.WasBoxDelivered(i);

            if (delivered)
                night1Boxes[i].SetActive(!onGroundFloor);
            else if (pickedUp)
                night1Boxes[i].SetActive(true);
            else
                night1Boxes[i].SetActive(onGroundFloor);
        }
    }
}
