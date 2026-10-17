using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractables
{
    public DialogueData dialogueData;

    public void Interact()
    {
        if (DialogueManager.Instance == null)
            return;

        if (DialogueManager.Instance.IsDialogueActive())
            return;

        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}