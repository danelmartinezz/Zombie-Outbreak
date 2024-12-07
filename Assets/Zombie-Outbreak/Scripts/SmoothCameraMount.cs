using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraMount : MonoBehaviour
{
    public Transform mount = null;
    public float speed = 5.0f;


    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, mount.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, mount.rotation, Time.deltaTime * speed);
    }
}
