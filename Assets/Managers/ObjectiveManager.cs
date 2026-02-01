using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Ordered objectives (Correct ID's)")]
    [SerializeField] private List<string> orderedObjectiveIds = new List<string>();

    [Header("Fake itmems (Still progresses objectives)")]
    [SerializeField] private List<FakeOption> fakeOptions = new List<FakeOption>();

    [Header("Ending State")]
    [SerializeField] public bool MadeMistake { get; private set; } = false;

    [Header("Final Choice Data")]
    [SerializeField] private int syringeObjectiveIndex = 6;

    public bool SyringeIsClean { get; private set; } = true;
    public bool SyringeWasPlaced { get; private set; } = false;
    public bool IngredientsAllCorrect => !MadeMistake;

    [Serializable]
    public class FakeOption
    {
        public int objectiveIndex; // which objective this index applies to
        public List<string> acceptedFakeIds; // wrong items that still progress
    }

    // used for end card choices
    [Serializable]
    public class ObjectiveChoice
    {
        public int objectiveIndex;
        public string requiredId;
        public string placedId;
        public bool wasCorrect;
    }

    public IReadOnlyList<ObjectiveChoice> Choices => choices;
    private readonly List<ObjectiveChoice> choices = new List<ObjectiveChoice>();

    public int CurrentIndex { get; private set; } = 0;
    public string CurrentObjectiveId => (CurrentIndex >= 0 && CurrentIndex < orderedObjectiveIds.Count) ? orderedObjectiveIds[CurrentIndex] : null;

    public event Action<int, string> OnObjectiveChanged;
    public event Action OnAllObjectivesComplete;

    public GameObject finalDoor;

    // cache: objectiveIndex -> fake IDs
    private Dictionary<int, HashSet<string>> fakeLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildFakeLookup();

        Debug.Log($"[ObjectiveManager] Awake instanceID={GetInstanceID()}");
    }

    private void BuildFakeLookup()
    {
        fakeLookup = new Dictionary<int, HashSet<string>>();

        foreach (var entry in fakeOptions)
        {
            if (entry == null) continue;
            if (entry.objectiveIndex < 0) continue;

            if (!fakeLookup.TryGetValue(entry.objectiveIndex, out var set))
            {
                set = new HashSet<string>(StringComparer.Ordinal);
                fakeLookup[entry.objectiveIndex] = set;
            }

            if (entry.acceptedFakeIds == null) continue;
            foreach (var id in entry.acceptedFakeIds)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    set.Add(id);
                }
            }
        }
    }

    public bool CompleteObjective(Objectiveitem item)
    {
        if (item == null || item.IsCompleted) return false;

        var requiredId = CurrentObjectiveId;
        if (string.IsNullOrEmpty(requiredId)) return false;

        var isCorrect = string.Equals(item.ObjectiveId, requiredId, StringComparison.Ordinal);
        var isAcceptedFake = !isCorrect && IsFakeAcceptedForCurrentObjective(item.ObjectiveId);

        if (!isCorrect && !isAcceptedFake) return false;

        if (isAcceptedFake)
        {
            MadeMistake = true;
            Debug.Log("Wrong but accepted item used for objective: " + CurrentIndex + " " + item.ObjectiveId);
        }

        choices.Add(new ObjectiveChoice
        {
            objectiveIndex = CurrentIndex,
            requiredId = requiredId,
            placedId = item.ObjectiveId,
            wasCorrect = isCorrect
        });

        if (CurrentIndex == syringeObjectiveIndex)
        {
            SyringeWasPlaced = true;
            SyringeIsClean = isCorrect;
        }

        item.MarkCompleted();
        CurrentIndex++;

        if (CurrentIndex >= orderedObjectiveIds.Count)
        {
            Debug.Log($"[ObjectiveManager] All Objectives Complete! instanceID={GetInstanceID()}");
            OnAllObjectivesComplete?.Invoke();
            finalDoor.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Current Objective: " + CurrentObjectiveId);
            OnObjectiveChanged?.Invoke(CurrentIndex, CurrentObjectiveId);
        }

        return true;
    }

    private bool IsFakeAcceptedForCurrentObjective(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        if (fakeLookup == null) BuildFakeLookup();

        return fakeLookup.TryGetValue(CurrentIndex, out var set) && set.Contains(itemId);
    }
}
