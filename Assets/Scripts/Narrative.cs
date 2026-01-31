using TMPro;
using UnityEngine;


public class Narrative : MonoBehaviour
{

    public TextAsset rawText;
    public TextMeshPro dialogueBox;
    string[] dialogueLines;

    void Start()
    {
        if (rawText == null)
        {
            Debug.LogError("Dialogue text not found");
            return;
        }

        //read entire file as string
        string fulltext = rawText.text;

        //read into array
        dialogueLines = fulltext.Split('\n');



    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            //GetNextDialogue();
        }
    }
}
