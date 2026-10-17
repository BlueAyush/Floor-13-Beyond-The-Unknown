using System.Collections;
using UnityEngine;
using TMPro;

public class BigDisplayElevator : MonoBehaviour
{
    public TMP_Text displayText;

    private string normalDisplay = "";
    private Coroutine messageCoroutine;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowMessage("GO BACK", 3f);
        }
    }
    // Shows the normal travel display

    public void ShowTravel(int fromFloor, int toFloor)
    {
        normalDisplay = FloorToText(fromFloor)
            + " <voffset=0.15em><size=180%>→</size></voffset> "
            + FloorToText(toFloor);

        // Only change the visible display if there isn't a temporary message
        if (messageCoroutine == null)
        {
            displayText.text = normalDisplay;
        }
    }

    public void ShowFloor(int floor)
    {
        normalDisplay = FloorToText(floor);

        if (messageCoroutine == null)
        {
            displayText.text = normalDisplay;
        }
    }

    // Shows a temporary story message
    public void ShowMessage(string message, float duration)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(MessageRoutine(message, duration));
    }

    private IEnumerator MessageRoutine(string message, float duration)
    {
        displayText.text = message;

        yield return new WaitForSeconds(duration);

        // Return to whatever the normal display was
        displayText.text = normalDisplay;

        messageCoroutine = null;
    }

    // Clears everything
    public void ClearDisplay()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        normalDisplay = "";
        displayText.text = "";
    }

    private string FloorToText(int floor)
    {
        if (floor == 0)
            return "G";

        return floor.ToString();
    }
}