using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object the camera will follow
    public float smoothSpeed = 0.125f; // The speed at which the camera will follow
    public Vector3 offset; // The offset from the target position

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset; // Calculate the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Smoothly move the camera towards the desired position
        transform.position = smoothedPosition; // Set the camera's position to the smoothed position
    }
}