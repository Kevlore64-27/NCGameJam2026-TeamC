using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class DialogueTextBox : MonoBehaviour
{
    [Header("TextBox Settings")]
    public TextMeshProUGUI dialogueBox;
    public float textSpeed;

    [HideInInspector] public int actIndex = 0;
    private int lineIndex;

    [Header("Text & Voice")]
    public List<string> dialogueLines;
    public List<string> idleLines;

    public bool isIdle = false;

    void OnEnable()
    {
        dialogueBox.text = string.Empty;
        StartDialogue();

    }
    private void Start()
    {

        ObjectiveManager.Instance.OnObjectiveChanged += ChangeAct;
    }

    void ChangeAct(int act, string item)
    {
        actIndex = act;
        isIdle = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isIdle)
            {
                if (dialogueBox.text == idleLines[lineIndex])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    dialogueBox.text = idleLines[lineIndex];
                }

            }
            else
            {
                if (dialogueBox.text == dialogueLines[lineIndex])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    dialogueBox.text = dialogueLines[lineIndex];
                }
            }
        }
    }

    void StartDialogue()
    {
        if (!isIdle)
        {
            lineIndex = 0;
            StartCoroutine(TypeLine());
        }
        else
        {
            lineIndex = Random.Range(0, Narrative.Instance.voiceLines[actIndex].idleClips.Length);
            StartCoroutine(TypeIdleLine());
        }
    }

    IEnumerator TypeLine()
    {
        Narrative.Instance.doctor.PlayOneShot(Narrative.Instance.voiceLines[actIndex].voiceClips[lineIndex]);
        foreach (char c in dialogueLines[lineIndex].ToCharArray())
        {
            dialogueBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator TypeIdleLine()
    {
        Narrative.Instance.doctor.PlayOneShot(Narrative.Instance.voiceLines[actIndex].idleClips[lineIndex]);
        foreach (char c in idleLines[lineIndex].ToCharArray())
        {
            dialogueBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        Narrative.Instance.doctor.Stop();

        if (isIdle)
        {
            gameObject.SetActive(false);
            return;
        }

        if (lineIndex < dialogueLines.Count - 1)
        {
            lineIndex++;
            dialogueBox.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            isIdle = true;
            gameObject.SetActive(false);
        }
    }
}
