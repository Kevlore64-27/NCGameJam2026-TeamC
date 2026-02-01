using System.Collections;
using UnityEngine;

public class Narrative : MonoBehaviour
{

    public static Narrative Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    [SerializeField] public AudioSource doctor;
    [SerializeField] public VoiceLine[] voiceLines;

    [SerializeField] DialogueTextBox dialogueTextBox;
    [SerializeField] TextLine[] textLines;

    [SerializeField] private FinalChoiceUI finalChoice;

    private string pendingEndingKey;

    private void Start()
    {
        ChangeAct(0, "Mortar");
        ObjectiveManager.Instance.OnObjectiveChanged += ChangeAct;
        ObjectiveManager.Instance.OnAllObjectivesComplete += ObjectivesComplete;
    }

    void ChangeAct(int act, string item)
    {
        dialogueTextBox.dialogueLines.Clear();
        for (int i = 0; i < textLines[act].textClips.Count; i++)
            dialogueTextBox.dialogueLines.Add(textLines[act].textClips[i]);

        dialogueTextBox.idleLines.Clear();
        for (int i = 0; i < textLines[act].idleClips.Count; i++)
            dialogueTextBox.idleLines.Add(textLines[act].idleClips[i]);

        Debug.Log("Showing text for objective: " + item);
    }

    void ObjectivesComplete()
    {
        ObjectiveManager.Instance.OnObjectiveChanged -= ChangeAct;
        ObjectiveManager.Instance.OnAllObjectivesComplete -= ObjectivesComplete;
        StartCoroutine(FinalAct());
    }


    IEnumerator FinalAct()
    {
        dialogueTextBox.actIndex++;
        dialogueTextBox.isIdle = false;

        dialogueTextBox.dialogueLines.Clear();
        for (int i = 0; i < textLines[dialogueTextBox.actIndex].textClips.Count; i++)
            dialogueTextBox.dialogueLines.Add(textLines[dialogueTextBox.actIndex].textClips[i]);

        dialogueTextBox.idleLines.Clear();
        for (int i = 0; i < textLines[dialogueTextBox.actIndex].idleClips.Count; i++)
            dialogueTextBox.idleLines.Add(textLines[dialogueTextBox.actIndex].idleClips[i]);
        Debug.Log("Showing text for 'Choice' narrative moment");

        dialogueTextBox.OnDialogueSequenceFinished -= OnFinalDialogueFinished;
        dialogueTextBox.OnDialogueSequenceFinished += OnFinalDialogueFinished;

        dialogueTextBox.gameObject.SetActive(true);

        yield return null;
    }

    private void OnFinalDialogueFinished()
    {
        dialogueTextBox.OnDialogueSequenceFinished -= OnFinalDialogueFinished;

        Debug.Log("Final dialogue finsiehd -> showing injection choices");

        if (finalChoice != null)
        {
            finalChoice.ShowChoicesFromNarrative();
        }
    }

    public void StartPostChoiceDialogue(int actToPlay, string endingKey)
    {
        pendingEndingKey = endingKey;

        dialogueTextBox.isIdle = false;
        dialogueTextBox.actIndex = actToPlay;

        // load lines...
        dialogueTextBox.dialogueLines.Clear();
        for (int i = 0; i < textLines[actToPlay].textClips.Count; i++)
            dialogueTextBox.dialogueLines.Add(textLines[actToPlay].textClips[i]);

        dialogueTextBox.idleLines.Clear();
        for (int i = 0; i < textLines[actToPlay].idleClips.Count; i++)
            dialogueTextBox.idleLines.Add(textLines[actToPlay].idleClips[i]);

        dialogueTextBox.OnDialogueSequenceFinished -= OnPostChoiceDialogueFinished;
        dialogueTextBox.OnDialogueSequenceFinished += OnPostChoiceDialogueFinished;

        dialogueTextBox.gameObject.SetActive(true);
    }

    private void OnPostChoiceDialogueFinished()
    {
        dialogueTextBox.OnDialogueSequenceFinished -= OnPostChoiceDialogueFinished;

        if (finalChoice != null)
            finalChoice.ShowEnding(pendingEndingKey); // call ShowEnding directly with the stored key
    }
}
