using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class CameraAspectRatio : MonoBehaviour
{
    public readonly float targetAspectRatio = 16f / 9f;
    [SerializeField] GameObject lookAtInFinalScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(UpdateAspectRatio());
    }

    IEnumerator UpdateAspectRatio()
    {
        Camera cam = GetComponent<Camera>();

        while (true)
        {
            float currentAspectRatio = (float)Screen.width / (float)Screen.height;

            float scaleHeight = currentAspectRatio / targetAspectRatio;

            Rect rect = cam.rect;

            if (scaleHeight < 1f)
            {
                rect.width = 1;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1f - scaleHeight) / 2;
            }
            else
            {
                float scaleWidth = 1f / scaleHeight;
                rect.width = scaleWidth;
                rect.height = 1;
                rect.x = (1f - scaleWidth) / 2;
                rect.y = 0;
            }

            cam.rect = rect;
            yield return new WaitForEndOfFrame();
        }
    }
}
