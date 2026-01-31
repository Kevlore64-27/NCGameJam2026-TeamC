using UnityEngine;

public class ObjectiveTable : MonoBehaviour
{
    [Header("Snap Points")]
    [SerializeField] private Transform[] snapPoints;

    [Header("Behaviours")]
    [SerializeField] private bool disablePickupOnPlace = true;

    private void OnTriggerEnter(Collider other)
    {
        Objectiveitem item = other.GetComponentInParent<Objectiveitem>();
        if (item == null) return;

        PickupItem pickup = other.GetComponentInParent<PickupItem>();
        if (pickup != null && !pickup.IsHeld) return;

        int indexBefore = ObjectiveManager.Instance.CurrentIndex;

        var accepted = ObjectiveManager.Instance.CompleteObjective(item);
        if (!accepted) return;

        var snap = GetSnapPoint(indexBefore);

        if (snap != null)
        {
            PlaceItem(item, pickup, snap);
        }
    }

    private Transform GetSnapPoint(int objectiveIndex)
    {
        if (snapPoints == null || snapPoints.Length == 0) return null;
        if (objectiveIndex < 0 || objectiveIndex >= snapPoints.Length) return null;
        return snapPoints[objectiveIndex];
    }

    private void PlaceItem(Objectiveitem item, PickupItem pickup, Transform snap)
    {
        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (disablePickupOnPlace && pickup != null)
        {
            Destroy(pickup);
        }

        item.transform.position = snap.position;
        item.transform.rotation = snap.rotation;
    }
}
