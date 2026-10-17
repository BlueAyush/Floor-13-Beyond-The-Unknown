using UnityEngine;

public class MonologueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;

    private bool triggered = false;

    public void TriggerMonologue()
    {
        if (triggered)
            return;

        if (DialogueManager.Instance == null)
            return;

        if (dialogueData == null)
            return;

        if (DialogueManager.Instance.IsDialogueActive())
            return;

        triggered = true;

        DialogueManager.Instance.StartDialogue(dialogueData);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TriggerMonologue();
        }
    }
}