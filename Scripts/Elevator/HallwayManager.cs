using UnityEngine;

public class HallwayManager : MonoBehaviour
{
    public static HallwayManager Instance { get; private set; }
    public int currentFloor = 1;
    public GameObject groundFloor;
    public GameObject hallwayV1;
    public GameObject hallwayV2;
    public GameObject hallwayV3;
    public GameObject hallwayV4;
    public GameObject hallwayV5;
    public GameObject hallwayV6;
    public GameObject hallwayV7;
    public GameObject hallwayV8;
    public GameObject hallwayV9;
    public GameObject hallwayV10;
    public GameObject hallwayV11;
    public GameObject hallwayV12;
    public GameObject hallwayV13;



    void Start()
    {
        groundFloor.SetActive(true);
    }

    private void Awake()
    {
        // Initialize the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SwitchFloors()
    {
        // Deactivate all hallways
        hallwayV1.SetActive(false);
        hallwayV2.SetActive(false);
        hallwayV3.SetActive(false);
        hallwayV4.SetActive(false);
        hallwayV5.SetActive(false);
        hallwayV6.SetActive(false);
        hallwayV7.SetActive(false);
        hallwayV8.SetActive(false);
        hallwayV9.SetActive(false);
        hallwayV10.SetActive(false);
        hallwayV11.SetActive(false);
        hallwayV12.SetActive(false);
        hallwayV13.SetActive(false);

        // Activate the current floor's hallway
        switch (currentFloor)
        {
            case 1:
                groundFloor.SetActive(true);
                break;
            case 2:
                hallwayV1.SetActive(true);
                break;
            case 3:
                hallwayV2.SetActive(true);
                break;
            case 4:
                hallwayV3.SetActive(true);
                break;
            case 5:
                hallwayV4.SetActive(true);
                break;
            case 6:
                hallwayV5.SetActive(true);
                break;
            case 7:
                hallwayV6.SetActive(true);
                break;
            case 8:
                hallwayV7.SetActive(true);
                break;
            case 9:
                hallwayV8.SetActive(true);
                break;
            case 10:
                hallwayV9.SetActive(true);
                break;
            case 11:
                hallwayV10.SetActive(true);
                break;
            case 12:
                hallwayV11.SetActive(true);
                break;
            case 13:
                hallwayV12.SetActive(true);
                break;
            default:
                Debug.LogWarning("Invalid floor number: " + currentFloor);
                break;
        }
    }
}