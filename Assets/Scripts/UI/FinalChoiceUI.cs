using TMPro;
using UnityEngine;

public class FinalChoiceUI : MonoBehaviour
{
    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;

    [Header("Ending Screen UI")]
    [SerializeField] private GameObject endingScreen;
    [SerializeField] private TMP_Text endingText;

    [Header("Syringe Step")]
    [Tooltip("0-based index of the syringe objective in ObjectiveManager.")]
    [SerializeField] private int syringeObjectiveIndex = 0;

    [Tooltip("ObjectiveId of the dirty syringe prefab.")]
    [SerializeField] private string dirtySyringeId = "Dirty Syringe";

    private ObjectiveManager manager;

    private void Awake()
    {
        if (choicePanel != null)
        {
            if (choicePanel != null) choicePanel.SetActive(false);
            if (endingScreen != null) endingScreen.SetActive(false);
        }
    }

    private void Start()
    {
        manager = ObjectiveManager.Instance != null ? ObjectiveManager.Instance : FindAnyObjectByType<ObjectiveManager>();
        if (manager != null)
        {
            manager.OnAllObjectivesComplete += ShowChoices;
        }
    }

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.OnAllObjectivesComplete -= ShowChoices;
        }
    }

    private void ShowChoices()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Time.timeScale = 0.0f;
    }

    public void ChooseInjectSelf()
    {
        Debug.Log("[FinalChoiceUI] Inject SELF clicked");
        ResolveEnding(true);
    }

    public void ChooseInjectDoctor()
    {
        Debug.Log("[FinalChoiceUI] Inject DOCTOR clicked");
        ResolveEnding(false);
    }

    private void ResolveEnding(bool injectSelf)
    {
        if (manager == null)
        {
            ShowEnding("ERROR: ObjectiveManager missing");
            return;
        }

        bool ingredientsCorrect = !manager.MadeMistake;
        bool syringeClean = GetSyringeIsCleanFromChoices();

        // Dirty syringe: both die (always)
        if (!syringeClean)
        {
            ShowEnding("BothDie_DirtySyringe");
            return;
        }

        // Clean syringe cases
        if (injectSelf)
        {
            if (ingredientsCorrect)
                ShowEnding("GoodEscape_SelfClean_AllCorrect");
            else
                ShowEnding("Die_SelfClean_WrongIngredients");
        }
        else
        {
            if (ingredientsCorrect)
                ShowEnding("DoctorEscapes_YouDie_Clean_AllCorrect");
            else
                ShowEnding("BothDie_Clean_WrongIngredients");
        }
    }

    private bool GetSyringeIsCleanFromChoices()
    {
        if (manager == null) return false;

        // If syringe was never placed / no choice recorded, treat as dirty (safe fail)
        if (manager.Choices == null) return false;

        foreach (var choice in manager.Choices)
        {
            if (choice.objectiveIndex == syringeObjectiveIndex)
            {
                // If they placed the dirty syringe ID, it's dirty. Otherwise assume clean.
                // (This lets you rename the clean ID freely; only dirty needs to match.)
                return !string.Equals(choice.placedId, dirtySyringeId, System.StringComparison.Ordinal);
            }
        }

        return false; // not found => treat as dirty
    }

    private void ShowEnding(string endingKey)
    {
        Debug.Log($"[FinalChoiceUI] ENDING: {endingKey}");

        // Hide choice panel
        if (choicePanel != null) choicePanel.SetActive(false);

        // Show black screen
        if (endingScreen != null) endingScreen.SetActive(true);

        // Set ending text
        if (endingText != null)
            endingText.text = FormatEndingText(endingKey);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Freeze game after choice (optional but usually desired)
        Time.timeScale = 0f;
    }

    private string FormatEndingText(string key)
    {
        return key switch
        {
            "GoodEscape_SelfClean_AllCorrect" =>
                "You inject yourself.\n\nThe serum works.\nYou escape.\n\nYOU SURVIVED.",

            "Die_SelfClean_WrongIngredients" =>
                "You inject yourself.\n\nThe serum fails.\n\nYOU DIED.",

            "DoctorEscapes_YouDie_Clean_AllCorrect" =>
                "You inject the plague doctor.\n\nHe escapes.\nYou collapse.\n\nYOU DIED.",

            "BothDie_Clean_WrongIngredients" =>
                "You inject the plague doctor.\n\nThe serum is flawed.\n\nYOU BOTH DIED.",

            "BothDie_DirtySyringe" =>
                "The syringe was contaminated.\n\nThe plague spreads.\n\nYOU BOTH DIED.",

            _ => $"ENDING:\n{key}"
        };
    }
}
