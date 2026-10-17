using System.Collections;
using TMPro;
using UnityEngine;

public class FloorDisplay : MonoBehaviour
{
    [Header("Displays")]
    public TextMeshPro buttonDisplay;

    public void SetFloor(int floor)
    {
        UpdateDisplays(FloorToString(floor));
    }

    public IEnumerator AnimateFloor(int startFloor, int targetFloor, float delay)
    {
        int step = startFloor < targetFloor ? 1 : -1;

        int currentFloor = startFloor;

        while (currentFloor != targetFloor)
        {
            UpdateDisplays(FloorToString(currentFloor));

            yield return new WaitForSeconds(delay);

            currentFloor += step;
        }

        UpdateDisplays(FloorToString(targetFloor));
    }

    void UpdateDisplays(string floorText)
    {
        if (buttonDisplay != null)
            buttonDisplay.text = floorText;
    }

    string FloorToString(int floor)
    {
        return floor == 0 ? "G" : floor.ToString();
    }
}