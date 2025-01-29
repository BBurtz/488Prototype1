using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookBehavior : MonoBehaviour
{
    public Transform orientation;
    public PlayerMovement player;

    public float sensX;
    public float sensY;

    public float xRotation;
    public float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // have the camera follow the players mouse
    void Update()
    {
        if (true)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}