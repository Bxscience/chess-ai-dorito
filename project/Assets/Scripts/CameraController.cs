using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform[] cameraAngles;
    int cameraSwitches = 0;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) cameraSwitches++;

        Transform targetAngle = cameraAngles[cameraSwitches % cameraAngles.Length];
        transform.position = targetAngle.position;
        transform.rotation = targetAngle.rotation;
    }
}
