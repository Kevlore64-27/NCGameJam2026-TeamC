using UnityEngine;

public class DoorKeyUnlock : MonoBehaviour
{
    [SerializeField] private Door door;

    private void Awake()
    {
        if (door == null)
        {
            door = GetComponentInParent<Door>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (door == null || !door.IsLocked) return;

        if (!other.CompareTag("Key")) return;

        var pickup = other.GetComponentInParent<PickupItem>();
        if (pickup != null && !pickup.IsHeld)
        {
            return;
        }

        door.Unlock();

        Destroy(other.gameObject);
    }
}
