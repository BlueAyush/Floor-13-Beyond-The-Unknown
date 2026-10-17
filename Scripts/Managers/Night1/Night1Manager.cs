using UnityEngine;

public class Night1Manager : MonoBehaviour
{
    [Header("Boxes")]
    public GameObject[] boxes;

    [Header("Glow Objects")]
    public GameObject[] boxGlows;

    public GameObject boxDeliveryZone;

    private bool[] boxPickedUp;
    private bool[] boxDelivered;

    private int currentBox = 0;



    void Awake()
    {
        if (NightManager.Instance != null &&
            NightManager.Instance.CurrentNight != 1)
        {
            return;
        }

        boxPickedUp = new bool[boxes.Length];
        boxDelivered = new bool[boxes.Length];
    }


    void Start()
    {
        if (NightManager.Instance != null &&
            NightManager.Instance.CurrentNight != 1)
        {
            return;
        }

        UpdateBoxGlow();

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("Talk to the receptionist");
        }
    }

    public bool CanPickup(GameObject box)
    {
        if (currentBox >= boxes.Length)
            return false;

        GameObject current = boxes[currentBox];

        if (box == current)
            return true;

        if (box.transform.IsChildOf(current.transform))
            return true;

        if (current.transform.IsChildOf(box.transform))
            return true;

        return false;
    }

    public void BoxPickedUp(GameObject box)
    {
        int index = GetBoxIndex(box);

        if (index >= 0)
        {
            boxPickedUp[index] = true;
        }
    }

    public void BoxDropped(GameObject box)
    {
        // Do nothing.
        // The box remains active wherever it was dropped.
    }

    public void BoxDelivered(GameObject box)
    {
        if (!CanPickup(box))
            return;

        int index = GetBoxIndex(box);

        if (index >= 0)
        {
            boxDelivered[index] = true;
        }

        Debug.Log("Box " + (currentBox + 1) + " delivered!");

        currentBox++;

        UpdateBoxGlow();

        if (currentBox >= boxes.Length)
        {
            Debug.Log("Night 1 - All boxes delivered!");

            boxDeliveryZone.SetActive(false);

            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.SetObjective("Go to bed");
            }
        }
    }

    int GetBoxIndex(GameObject box)
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            GameObject assignedBox = boxes[i];

            if (box == assignedBox)
                return i;

            if (box.transform.IsChildOf(assignedBox.transform))
                return i;

            if (assignedBox.transform.IsChildOf(box.transform))
                return i;
        }

        return -1;
    }

    public bool WasBoxPickedUp(int index)
    {
        if (boxPickedUp == null ||
            index < 0 ||
            index >= boxPickedUp.Length)
            return false;

        return boxPickedUp[index];
    }

    public bool WasBoxDelivered(int index)
    {
        if (boxDelivered == null ||
            index < 0 ||
            index >= boxDelivered.Length)
            return false;

        return boxDelivered[index];
    }

    void UpdateBoxGlow()
    {
        // Turn all glows off
        for (int i = 0; i < boxGlows.Length; i++)
        {
            if (boxGlows[i] != null)
                boxGlows[i].SetActive(false);
        }

        // Turn on current box glow
        if (currentBox < boxGlows.Length)
        {
            if (boxGlows[currentBox] != null)
                boxGlows[currentBox].SetActive(true);
        }
    }
}