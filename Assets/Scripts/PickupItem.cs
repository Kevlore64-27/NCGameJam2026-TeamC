using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupItem : MonoBehaviour
{
    private Rigidbody rb;
    private FirstPersonController controller;
    private Transform objectGrabPointTransform;

    private int defaultLayer;
    private int grabLayer;

    private float skin = 0.02f; // keeps a tiny gap from surface
    private float maxMovePerStep = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        defaultLayer = gameObject.layer;
        grabLayer = LayerMask.NameToLayer("PickUp");
    }

    public void Grab(Transform grabPointTransform, FirstPersonController player)
    {
        objectGrabPointTransform = grabPointTransform;
        controller = player;
        SetNewLayer(gameObject, grabLayer);
        rb.useGravity = false;
        //rb.isKinematic = true; // no physics while held
    }

    public void Drop()
    {
        objectGrabPointTransform = null;
        controller = null;
        SetNewLayer(gameObject, defaultLayer);
        rb.useGravity = true;
        //rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        //if (objectGrabPointTransform != null)
        //{
        //    Vector3 velocityCompensation = controller.PlayerVelocity * Time.fixedDeltaTime;
        //    //Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * 10f);
        //    Vector3 newPosition = Vector3.Lerp(rb.position, objectGrabPointTransform.position + velocityCompensation, Time.fixedDeltaTime * 10f);
        //    rb.MovePosition(newPosition);
        //
        //    Quaternion newRotation = Quaternion.Lerp(transform.rotation, objectGrabPointTransform.rotation, Time.fixedDeltaTime * 10f);
        //    rb.MoveRotation(newRotation);
        //}

        if (objectGrabPointTransform != null)
        {
            Vector3 velocityComp = controller.PlayerVelocity * Time.fixedDeltaTime;
            Vector3 desiredPos = Vector3.Lerp(rb.position, objectGrabPointTransform.position + velocityComp, Time.fixedDeltaTime * 10.0f);
            Vector3 move = desiredPos - rb.position;

            float moveDist = move.magnitude;
            if (moveDist > maxMovePerStep)
            {
                move = move.normalized * maxMovePerStep;
            }

            moveDist = move.magnitude;
            if (moveDist > 0.0001f)
            {
                Vector3 dir = move / moveDist;

                if (rb.SweepTest(dir, out RaycastHit hit, moveDist))
                {
                    float allowed = Mathf.Max(0.0f, hit.distance - skin);
                    rb.MovePosition(rb.position + dir * allowed);
                }
                else
                {
                    rb.MovePosition(rb.position + move);
                }
            }

            Quaternion newRot = Quaternion.Lerp(rb.rotation, objectGrabPointTransform.rotation, Time.fixedDeltaTime * 10.0f);
            rb.MoveRotation(newRot);
        }
    }

    private static void SetNewLayer(GameObject go, int newLayer)
    {
        go.layer = newLayer;
        foreach (Transform child in go.transform)
        {
            SetNewLayer(child.gameObject, newLayer);
        }
    }
}
