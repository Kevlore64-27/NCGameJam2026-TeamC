using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool startsLocked = true;

    [Header("Door Parameters")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90.0f;
    [SerializeField] private float openSpeed = 180.0f;
    [SerializeField] private float closeSpeed = 270.0f;

    [Header("Audio")]
    [SerializeField] private AudioClip unlockSfx;
    [SerializeField] private AudioClip openSfx;
    [SerializeField] private AudioClip closeSfx;
    [SerializeField] private AudioClip rattleSfx;

    public bool IsLocked => isLocked;

    private bool isLocked;
    private bool isOpen;

    private Quaternion closedRot;
    private Quaternion openRot;

    private void Awake()
    {
        isLocked = startsLocked;

        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        closedRot = doorPivot.localRotation;
        openRot = closedRot * Quaternion.Euler(0.0f, openAngle, 0.0f);
    }

    public void Unlock()
    {
        if (!isLocked)
        {
            return;
        }

        isLocked = false;

        if (unlockSfx != null)
        {
            AudioManager.Instance.PlaySfx(unlockSfx);
        }

        Debug.Log("Door unlocked!");
    }

    public void Interact()
    {
        if (isLocked)
        {
            if (rattleSfx != null)
            {
                AudioManager.Instance.PlaySfx(rattleSfx);
            }
            Debug.Log("Door locked...");
            return;
        }

        isOpen = !isOpen;

        if (isOpen && openSfx != null)
        {
            AudioManager.Instance.PlaySfx(openSfx);
        }
        else if (!isOpen && closeSfx != null)
        {
            AudioManager.Instance.PlaySfx(closeSfx);
        }
    }

    private void Update()
    {
        Quaternion target = isOpen ? openRot : closedRot;
        float speed = isOpen ? openSpeed : closeSpeed;

        doorPivot.localRotation = Quaternion.RotateTowards(doorPivot.localRotation, target, speed * Time.deltaTime);
    }
}
