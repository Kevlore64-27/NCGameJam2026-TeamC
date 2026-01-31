using UnityEngine;

public class PlayerGrabSystem : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabDistance = 3f;
    [SerializeField] private FirstPersonController controller;
    private PickupItem heldItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, grabDistance))
                {
                    if (hit.collider.TryGetComponent(out PickupItem pickup))
                    {
                        heldItem = pickup;
                        heldItem.Grab(grabPoint, controller);
                    }
                }
            }
            else
            {
                heldItem.Drop();
                heldItem = null;
            }
        }
    }
}
