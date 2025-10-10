using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCam : NetworkBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Transform orientation;

    [SerializeField] private float xRot;
    [SerializeField] private float yRot;

    private void Start()
    {
        if(!IsOwner) return;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
        if(!IsOwner) return;

        //mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, -90, 90);

        // orientation
        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }
}
