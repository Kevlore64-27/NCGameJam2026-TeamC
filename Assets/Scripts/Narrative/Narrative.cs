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

    private void Start()
    {
        ChangeAct(0, "Mortar");
        ObjectiveManager.Instance.OnObjectiveChanged += ChangeAct;
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
}
