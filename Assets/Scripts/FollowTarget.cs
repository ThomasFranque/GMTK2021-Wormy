using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private static FollowTarget instance;
    public static FollowTarget Current => instance;

    public Transform followTarget;
    [SerializeField] private float rotationPower = 1f;
    private Vector2 lookRot;

    private void Awake()
    {
        instance = this;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        if (followTarget)
        {
            transform.position = followTarget.position;
        }

        lookRot = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Rotate();
    }

    void Rotate()
    {
        transform.rotation *= Quaternion.AngleAxis(lookRot.x * rotationPower, Vector3.up);

        transform.rotation *= Quaternion.AngleAxis(-lookRot.y * rotationPower, Vector3.right);

        var angles = transform.localEulerAngles;
        angles.z = 0;

        var angle = transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

    }
}
