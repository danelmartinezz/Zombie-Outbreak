using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public float speed = 0.0f;

    private Animator _controller = null;

    private void Start()
    {
        _controller = GetComponent<Animator>();
    }

    private void Update()
    {
        _controller.SetFloat("Speed", speed);
    }
}
