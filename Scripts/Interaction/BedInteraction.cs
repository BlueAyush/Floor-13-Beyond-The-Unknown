using UnityEngine;

public class BedInteraction : MonoBehaviour, IInteractables
{
    public void Interact()
    {
        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.IsCurrentObjective("Go to bed"))
        {
            Debug.Log("Cannot sleep yet.");
            return;
        }

        Debug.Log("Player is going to bed.");

        ObjectiveManager.Instance.ClearObjective();

        if (SleepSequence.Instance != null)
        {
            SleepSequence.Instance.StartSleep();
        }
    }
}