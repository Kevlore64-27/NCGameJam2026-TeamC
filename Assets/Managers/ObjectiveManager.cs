using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Ordered objectives (ID's)")]
    [SerializeField] private List<string> orderedObjectiveIds = new List<string>();

    public int CurrentIndex { get; private set; } = 0;
    public string CurrentObjectiveId => (CurrentIndex >= 0 && CurrentIndex < orderedObjectiveIds.Count) ? orderedObjectiveIds[CurrentIndex] : null;

    public event Action<int, string> OnObjectiveChanged;
    public event Action OnAllObjectivesComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Current Objective: " + CurrentObjectiveId);
    }

    public bool CompleteObjective(Objectiveitem item)
    {
        if (item == null || item.IsCompleted) return false;

        var requiredId = CurrentObjectiveId;
        if (string.IsNullOrEmpty(requiredId)) return false;

        if (!string.Equals(item.ObjectiveId, requiredId, StringComparison.Ordinal)) return false;

        item.MarkCompleted();
        CurrentIndex++;

        if (CurrentIndex >= orderedObjectiveIds.Count)
        {
            Debug.Log("All Objectives Complete!");
            OnAllObjectivesComplete?.Invoke();
        }
        else
        {
            Debug.Log("Current Objective: " + CurrentObjectiveId);
            OnObjectiveChanged?.Invoke(CurrentIndex, CurrentObjectiveId);
        }

        return true;
    }
}
