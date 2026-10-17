using UnityEngine;

public enum DialogueType
{
    Conversation,
    Monologue,
    ShortThought
}

[System.Serializable]
public class DialogueChoice
{
    [Header("Choice Text")]
    public string choiceText;

    [Header("Next Dialogue Line")]
    public int nextLineIndex;
}

[System.Serializable]
public class DialogueCondition
{
    public string flagName;
    public bool requiredValue = true;
}

public enum DialogueEventType
{
    SetStoryFlag,
    SetObjective,
    ClearObjective
}

[System.Serializable]
public class DialogueEvent
{
    public DialogueEventType eventType;

    [Header("Story Flag")]
    public string flagName;

    public bool flagValue = true;

    [Header("Objective")]
    [TextArea(1, 3)]
    public string objectiveText;
}

[System.Serializable]
public class DialogueLine
{
    [Header("Line Settings")]
    public DialogueType dialogueType;

    [Header("Speaker")]
    public string speakerName;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string text;

    [Header("Voice")]
    public AudioClip voiceClip;

    [Header("Choices")]
    public DialogueChoice[] choices;

    [Header("Condition")]
    public DialogueCondition condition;

    [Header("Events")]
    public DialogueEvent[] events;

    [Header("Next Line")]
    public int nextLineIndex = -1;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Floor 13/Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}