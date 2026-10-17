
using System.Collections;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [Header("References")]
    public FloorManager floorManager;
    public ElevatorDoor elevatorDoor;
    public FloorDisplay floorDisplay;
    public BigDisplayElevator bigDisplay;

    private ElevatorButtonLight activeButton;

    [Header("Settings")]
    public float doorCloseDelay = 2f;
    public float displaySpeed = 0.4f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource travelHumSource;
    [SerializeField] private AudioSource elevatorFanSource;
    [SerializeField] private AudioSource elevatorSFXSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip travelHumClip;
    [SerializeField] private AudioClip shortTravelHumClip;
    [SerializeField] private AudioClip elevatorFanClip;
    [SerializeField] private AudioClip elevatorDingClip;

    [Header("Travel Timing")]
    [SerializeField] private int shortTripMaxFloors = 2;
    [SerializeField] private float shortTripDuration = 4f;
    [SerializeField] private float secondsPerFloor = 2f;
    [SerializeField] private float maxLongTripDuration = 12f;

    private bool isMoving = false;

    void Start()
    {
        if (floorManager != null && floorDisplay != null)
            floorDisplay.SetFloor(floorManager.CurrentFloor);

        if (travelHumSource != null)
        {
            travelHumSource.loop = false;
            travelHumSource.pitch = 1f;
        }
    }

    public void GoToFloor(int targetFloor, ElevatorButtonLight button)
    {
        if (isMoving)
            return;

        if (floorManager == null || elevatorDoor == null ||
            floorDisplay == null)
        {
            Debug.LogError(
                "ElevatorManager is missing a required reference!"
            );
            return;
        }

        if (targetFloor == floorManager.CurrentFloor)
            return;

        activeButton = button;

        if (activeButton != null)
            activeButton.StartFlicker();

        if (bigDisplay != null)
        {
            bigDisplay.ShowTravel(
                floorManager.CurrentFloor,
                targetFloor
            );
        }

        StartCoroutine(TravelToFloor(targetFloor));
    }

    IEnumerator TravelToFloor(int targetFloor)
    {
        isMoving = true;

        //elevator Fan sound
        if (elevatorFanSource != null)
        {
            elevatorFanSource.clip = elevatorFanClip;
            elevatorFanSource.loop = true;
            elevatorFanSource.Play();
        }

        // Close the doors before travel.
        elevatorDoor.CloseDoors();

        yield return new WaitForSeconds(doorCloseDelay);

        int startFloor = floorManager.CurrentFloor;
        int floorDistance = Mathf.Abs(targetFloor - startFloor);

        // Choose the appropriate motor clip.
        bool isShortTrip = floorDistance <= shortTripMaxFloors;

        AudioClip selectedClip = isShortTrip &&
                                 shortTravelHumClip != null
            ? shortTravelHumClip
            : travelHumClip;

        // Determine how long the ride should take.
        float travelDuration;

        if (isShortTrip && shortTravelHumClip != null)
        {
            travelDuration = shortTripDuration;
        }
        else
        {
            travelDuration = Mathf.Min(
                floorDistance * secondsPerFloor,
                maxLongTripDuration
            );
        }

        // Start the selected motor clip.
        if (travelHumSource != null && selectedClip != null)
        {
            travelHumSource.Stop();
            travelHumSource.clip = selectedClip;
            travelHumSource.loop = false;

            // Speed up the long recording to fit the ride duration.
            travelHumSource.pitch = Mathf.Max(
                0.1f,
                selectedClip.length / travelDuration
            );

            travelHumSource.Play();
        }

        // Synchronize the floor display with the planned ride.
        float delayPerFloor = travelDuration / floorDistance;

        yield return StartCoroutine(
            floorDisplay.AnimateFloor(
                startFloor,
                targetFloor,
                delayPerFloor
            )
        );

        // Wait for the motor to finish.
        if (travelHumSource != null && travelHumSource.isPlaying)
        {
            yield return new WaitWhile(
                () => travelHumSource != null &&
                      travelHumSource.isPlaying
            );
        }

        // Update the destination floor.
        floorManager.SetFloor(targetFloor);

        if (bigDisplay != null)
            bigDisplay.ShowFloor(targetFloor);

        if (activeButton != null)
        {
            activeButton.StopFlicker();
            activeButton = null;
        }

        if (travelHumSource != null)
            travelHumSource.pitch = 1f;

        // Arrival DING.
        if (elevatorSFXSource != null && elevatorDingClip != null)
            elevatorSFXSource.PlayOneShot(elevatorDingClip);

        yield return new WaitForSeconds(0.35f);

        //stop elevator fan sound
        if (elevatorFanSource != null)
        {
            elevatorFanSource.Stop();
        }
        // Allow the doors to open.
        isMoving = false;
        elevatorDoor.OpenDoors();
    }

    public void CallElevator()
    {
        if (isMoving || elevatorDoor == null)
            return;

        elevatorDoor.OpenDoors();
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}