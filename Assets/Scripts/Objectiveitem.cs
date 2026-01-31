using UnityEngine;

public class Objectiveitem : MonoBehaviour
{
    [field: SerializeField] public string ObjectiveId { get; private set; }

    public bool IsCompleted { get; private set; }

    public void MarkCompleted()
    {
        IsCompleted = true;
    }
}
