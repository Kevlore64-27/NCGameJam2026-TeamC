using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject endStatsPanel;
    [SerializeField] private TMP_Text statsText;

    private ObjectiveManager mgr;
    private bool subscribed;

    private void Awake()
    {
        if (endStatsPanel != null)
        {
            endStatsPanel.SetActive(false);
        }
    }

    private void Start()
    {
        mgr = ObjectiveManager.Instance != null ? ObjectiveManager.Instance : FindAnyObjectByType<ObjectiveManager>();
    }

    private void Update()
    {
        // Keep trying until we find the ObjectiveManager and subscribe
        if (subscribed) return;

        mgr = ObjectiveManager.Instance != null ? ObjectiveManager.Instance : FindAnyObjectByType<ObjectiveManager>();
        if (mgr == null) return;

        mgr.OnAllObjectivesComplete += ShowEndCard;
        subscribed = true;

        Debug.Log($"[EndCardUI] Subscribed. ObjectiveManager instanceID={mgr.GetInstanceID()}");
    }

    private void OnDestroy()
    {
        if (subscribed && mgr != null)
            mgr.OnAllObjectivesComplete -= ShowEndCard;
    }

    private void ShowEndCard()
    {
        Debug.Log("End card showing");

        if (endStatsPanel != null)
        {
            endStatsPanel.SetActive(true);
        }

        if (mgr == null || statsText == null) return;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine(mgr.MadeMistake ? "Ending: BAD" : "Ending: GOOD");
        sb.AppendLine();

        foreach (var choice in mgr.Choices)
        {
            string mark = choice.wasCorrect ? "GOOD" : "BAD";
            sb.AppendLine($"{mark} Step {choice.objectiveIndex + 1}: placed {choice.placedId} (needed {choice.requiredId})");
        }

        statsText.text = sb.ToString();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0.0f;
    }
}
