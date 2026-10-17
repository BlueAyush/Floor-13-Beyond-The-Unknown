
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Player Movement")]
    public FirstPersonController playerController;

    [Header("Dialogue Animation")]
    public DialogueAnimationController dialogueAnimation;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [Header("Thought UI")]
    public GameObject thoughtUI;
    public TMP_Text thoughtText;

    [Header("Thought Settings")]
    public float monologueDisplayDuration = 2.5f;
    public float shortThoughtDisplayDuration = 1.5f;

    [Header("Choices")]
    public GameObject choicesPanel;
    public Button choiceButtonPrefab;

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;

    private DialogueData currentDialogue;
    private int currentLineIndex;

    private bool dialogueActive;
    private bool isTyping;
    private bool waitForKeyRelease;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        Instance = this;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (thoughtUI != null)
            thoughtUI.SetActive(false);

        if (choicesPanel != null)
            choicesPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    void Update()
    {
        if (!dialogueActive)
            return;

        // Wait until the previous input is released.
        if (waitForKeyRelease)
        {
            if (!Input.GetKey(KeyCode.E) &&
                !Input.GetKey(KeyCode.Space)
                )
            {
                waitForKeyRelease = false;
            }

            return;
        }

        // Don't advance dialogue while choices are displayed.
        if (choicesPanel != null && choicesPanel.activeSelf)
            return;

        // E, Spacebar, or left mouse click.
        bool advancePressed =
            Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.Space);
            

        if (!advancePressed)
            return;

        // First press finishes the typewriter effect.
        if (isTyping)
        {
            FinishTyping();
            waitForKeyRelease = true;
            return;
        }

        // Otherwise, move to the next line.
        NextLine();
        waitForKeyRelease = true;
    }


    // --------------------------------------------------
    // START DIALOGUE
    // --------------------------------------------------

    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null ||
            dialogue.lines == null ||
            dialogue.lines.Length == 0)
        {
            return;
        }

        StopTypingCoroutine();

        currentDialogue = dialogue;
        currentLineIndex = 0;

        dialogueActive = true;
        isTyping = false;

        // Prevent the interaction key from advancing immediately.
        waitForKeyRelease = true;

        if (dialogueAnimation != null)
            dialogueAnimation.StartTalking();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ShowCurrentLine();
    }

    // --------------------------------------------------
    // DISPLAY CURRENT LINE
    // --------------------------------------------------

    private void ShowCurrentLine()
    {
        while (dialogueActive &&
               currentDialogue != null &&
               currentLineIndex >= 0 &&
               currentLineIndex < currentDialogue.lines.Length)
        {
            DialogueLine line = GetCurrentLine();

            if (line == null)
            {
                currentLineIndex++;
                continue;
            }

            // Skip lines whose conditions are not satisfied.
            if (!IsConditionMet(line))
            {
                currentLineIndex++;
                continue;
            }

            StopTypingCoroutine();

            isTyping = false;

            ConfigureLineUI(line);
            HideChoices();

            // Events execute once, when this line is displayed.
            ExecuteEvents(line);

            typingCoroutine = StartCoroutine(TypeLine(line.text));
            return;
        }

        EndDialogue();
    }

    private void ConfigureLineUI(DialogueLine line)
    {
        bool isConversation =
            line.dialogueType == DialogueType.Conversation;

        if (playerController != null)
        {
            if (isConversation)
                playerController.LockMovement();
            else
                playerController.UnlockMovement();
        }

        if (isConversation)
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);

            if (thoughtUI != null)
                thoughtUI.SetActive(false);

            if (speakerText != null)
                speakerText.text = line.speakerName;
        }
        else
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            if (thoughtUI != null)
                thoughtUI.SetActive(true);

            if (speakerText != null)
                speakerText.text = "";
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --------------------------------------------------
    // TYPEWRITER
    // --------------------------------------------------

    private IEnumerator TypeLine(string text)
    {
        DialogueLine line = GetCurrentLine();

        if (line == null)
            yield break;

        bool isConversation =
            line.dialogueType == DialogueType.Conversation;

        bool isShortThought =
            line.dialogueType == DialogueType.ShortThought;

        TMP_Text targetText =
            isConversation ? dialogueText : thoughtText;

        if (targetText == null)
        {
            isTyping = false;
            typingCoroutine = null;
            yield break;
        }

        targetText.text = "";

        // Short thoughts appear instantly.
        if (isShortThought)
        {
            targetText.text = text;

            isTyping = false;

            yield return new WaitForSeconds(
                shortThoughtDisplayDuration
            );

            if (!dialogueActive)
                yield break;

            if (thoughtUI != null)
                thoughtUI.SetActive(false);

            targetText.text = "";
            typingCoroutine = null;

            NextLine();

            yield break;
        }

        isTyping = true;

        float delay = Mathf.Max(0f, typingSpeed);

        foreach (char letter in text)
        {
            if (!dialogueActive)
                yield break;

            targetText.text += letter;

            if (delay > 0f)
                yield return new WaitForSeconds(delay);
            else
                yield return null;
        }

        isTyping = false;
        typingCoroutine = null;

        if (isConversation)
        {
            ShowChoices();
        }
        else
        {
            // Monologue stays visible for its configured duration.
            yield return new WaitForSeconds(
                monologueDisplayDuration
            );

            if (!dialogueActive)
                yield break;

            if (thoughtUI != null)
                thoughtUI.SetActive(false);

            targetText.text = "";

            
            NextLine();
            
        }
    }

    private void FinishTyping()
    {
        if (!isTyping || currentDialogue == null)
            return;

        DialogueLine line = GetCurrentLine();

        if (line == null)
            return;

        StopTypingCoroutine();

        bool isConversation =
            line.dialogueType == DialogueType.Conversation;

        TMP_Text targetText =
            isConversation ? dialogueText : thoughtText;

        if (targetText != null)
            targetText.text = line.text;

        isTyping = false;

        if (isConversation)
        {
            ShowChoices();
        }
        else
        {
            // Continue the normal thought display.
            typingCoroutine = StartCoroutine(
                FinishThoughtAfterTyping(targetText, line.dialogueType)
            );
        }
    }

    private IEnumerator FinishThoughtAfterTyping(
        TMP_Text targetText,
        DialogueType type)
    {
        float duration = type == DialogueType.ShortThought
            ? shortThoughtDisplayDuration
            : monologueDisplayDuration;

        yield return new WaitForSeconds(duration);

        if (!dialogueActive)
            yield break;

        if (thoughtUI != null)
            thoughtUI.SetActive(false);

        if (targetText != null)
            targetText.text = "";

        typingCoroutine = null;

        NextLine();
    }

    // --------------------------------------------------
    // CHOICES
    // --------------------------------------------------

    private void ShowChoices()
    {
        DialogueLine line = GetCurrentLine();

        if (line == null)
            return;

        if (line.choices == null || line.choices.Length == 0)
        {
            if (playerController != null)
                playerController.UnlockLook();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            return;
        }

        if (playerController != null)
            playerController.LockLook();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (choicesPanel == null || choiceButtonPrefab == null)
            return;

        choicesPanel.SetActive(true);

        foreach (DialogueChoice choice in line.choices)
        {
            if (choice == null)
                continue;

            DialogueChoice capturedChoice = choice;

            Button button = Instantiate(
                choiceButtonPrefab,
                choicesPanel.transform
            );

            TMP_Text buttonText =
                button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = capturedChoice.choiceText;

            button.onClick.AddListener(
                () => SelectChoice(capturedChoice.nextLineIndex)
            );
        }
    }

    private void SelectChoice(int targetIndex)
    {
        if (!dialogueActive || currentDialogue == null)
            return;

        DialogueLine currentLine = GetCurrentLine();

        if (currentLine == null)
            return;

        if (playerController != null)
            playerController.UnlockLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentLine.choices != null)
        {
            foreach (DialogueChoice choice in currentLine.choices)
            {
                if (choice != null &&
                    choice.nextLineIndex == targetIndex)
                {
                    if (DialogueLog.Instance != null)
                    {
                        DialogueLog.Instance.AddLine(
                            "You",
                            choice.choiceText
                        );
                    }

                    break;
                }
            }
        }

        ClearChoiceButtons();

        if (targetIndex < 0 ||
            targetIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        currentLineIndex = targetIndex;
        ShowCurrentLine();
    }

    private void HideChoices()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);

        ClearChoiceButtons();

        if (playerController != null)
            playerController.UnlockLook();
    }

    private void ClearChoiceButtons()
    {
        if (choicesPanel == null)
            return;

        for (int i = choicesPanel.transform.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(choicesPanel.transform.GetChild(i).gameObject);
        }
    }

    // --------------------------------------------------
    // CONDITIONS
    // --------------------------------------------------

    private bool IsConditionMet(DialogueLine line)
    {
        if (line.condition == null ||
            string.IsNullOrEmpty(line.condition.flagName))
        {
            return true;
        }

        if (StoryFlags.Instance == null)
            return false;

        bool flagValue = StoryFlags.Instance.GetFlag(
            line.condition.flagName
        );

        return flagValue == line.condition.requiredValue;
    }

    // --------------------------------------------------
    // EVENTS
    // --------------------------------------------------

    private void ExecuteEvents(DialogueLine line)
    {
        if (line.events == null)
            return;

        foreach (DialogueEvent dialogueEvent in line.events)
        {
            if (dialogueEvent == null)
                continue;

            switch (dialogueEvent.eventType)
            {
                case DialogueEventType.SetStoryFlag:

                    if (StoryFlags.Instance != null)
                    {
                        StoryFlags.Instance.SetFlag(
                            dialogueEvent.flagName,
                            dialogueEvent.flagValue
                        );
                    }

                    break;

                case DialogueEventType.SetObjective:

                    if (ObjectiveManager.Instance != null)
                    {
                        ObjectiveManager.Instance.SetObjective(
                            dialogueEvent.objectiveText
                        );
                    }

                    break;

                case DialogueEventType.ClearObjective:

                    if (ObjectiveManager.Instance != null)
                    {
                        ObjectiveManager.Instance.ClearObjective();
                    }

                    break;
            }
        }
    }

    // --------------------------------------------------
    // ADVANCE DIALOGUE
    // --------------------------------------------------

    private void NextLine()
    {
        if (!dialogueActive || currentDialogue == null)
            return;

        DialogueLine currentLine = GetCurrentLine();

        if (currentLine == null)
        {
            EndDialogue();
            return;
        }

        AddCurrentLineToLog();

        int nextIndex = currentLine.nextLineIndex;

        // -1 ends the dialogue. Events have already run
        // when the current line was displayed.
        if (nextIndex < 0 ||
            nextIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        currentLineIndex = nextIndex;
        ShowCurrentLine();
    }

    private DialogueLine GetCurrentLine()
    {
        if (currentDialogue == null ||
            currentDialogue.lines == null ||
            currentLineIndex < 0 ||
            currentLineIndex >= currentDialogue.lines.Length)
        {
            return null;
        }

        return currentDialogue.lines[currentLineIndex];
    }

    private void AddCurrentLineToLog()
    {
        DialogueLine line = GetCurrentLine();

        if (line == null || DialogueLog.Instance == null)
            return;

        DialogueLog.Instance.AddLine(
            line.speakerName,
            line.text
        );
    }

    // --------------------------------------------------
    // END DIALOGUE
    // --------------------------------------------------

    private void StopTypingCoroutine()
    {
        if (typingCoroutine == null)
            return;

        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        StopTypingCoroutine();

        if (playerController != null)
        {
            playerController.UnlockMovement();
            playerController.UnlockLook();
        }

        ClearChoiceButtons();

        if (choicesPanel != null)
            choicesPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (thoughtUI != null)
            thoughtUI.SetActive(false);

        if (speakerText != null)
            speakerText.text = "";

        isTyping = false;
        dialogueActive = false;

        waitForKeyRelease = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (dialogueAnimation != null)
            dialogueAnimation.StopTalking();

        currentDialogue = null;
        currentLineIndex = 0;
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }

    // --------------------------------------------------
    // SLEEP MONOLOGUE
    // --------------------------------------------------

    public IEnumerator PlaySleepMonologue(string text)
    {
        if (thoughtUI != null)
            thoughtUI.SetActive(true);

        if (thoughtText != null)
            thoughtText.text = "";

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float delay = Mathf.Max(0f, typingSpeed);

        foreach (char letter in text)
        {
            if (thoughtText != null)
                thoughtText.text += letter;

            if (delay > 0f)
                yield return new WaitForSeconds(delay);
            else
                yield return null;
        }

        yield return new WaitForSeconds(monologueDisplayDuration);

        if (thoughtUI != null)
            thoughtUI.SetActive(false);

        if (thoughtText != null)
            thoughtText.text = "";
    }
}
