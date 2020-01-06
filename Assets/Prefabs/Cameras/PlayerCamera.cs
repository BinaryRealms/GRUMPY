using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform playerShip;

    public float smoothCameraSpeed = 0.1f;
    public Vector3 cameraOffset;
    public Vector3 velocity;
        
    void FixedUpdate()
    {
        Vector3 desiredPosition = playerShip.position + cameraOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothCameraSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(playerShip);
    }
}
