using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text currentObjectiveText;

    private string currentObjective = "";

    private void Awake()
    {
        Instance = this;

    }

    public void SetObjective(string objective)
    {
        currentObjective = objective;

        currentObjectiveText.text =
            "Current Objective:\n" + objective;
    }

    public void ClearObjective()
    {
        currentObjective = "";

        currentObjectiveText.text =
            "Current Objective:\nNone";
    }

    public bool IsCurrentObjective(string objective)
    {
        return currentObjective == objective;
    }

    public void CompleteObjective()
    {
        currentObjectiveText.text = "Current Objective:\nComplete!";
    }
}