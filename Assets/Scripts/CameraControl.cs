using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float speed = 10f;
    public float smoothTime = 0.2f;
    public float reboundDistance = 0.5f;
    public float reboundReturnTime = 0.2f;
    Vector3 currentVelocity = Vector3.zero;
    Vector3 velocitySmooth = Vector3.zero;

    Vector3 lastInputDir = Vector3.zero;

    private Vector3 reboundOffset = Vector3.zero;
    private Vector3 reboundVelocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDir = InputDirection();
        Vector3 targetVelocity = inputDir.normalized * speed;

        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocitySmooth, smoothTime);

        if (inputDir != Vector3.zero)
        {
            lastInputDir = inputDir.normalized;
        }

        if (inputDir == Vector3.zero && currentVelocity.magnitude < 0.3f)
        {
            if (reboundOffset == Vector3.zero && lastInputDir != Vector3.zero)
            {
                reboundOffset = -lastInputDir * reboundDistance;
                lastInputDir = Vector3.zero;
            }
            reboundOffset = Vector3.SmoothDamp(reboundOffset, Vector3.zero, ref reboundVelocity, reboundReturnTime);
        }
        else
        {
            reboundOffset = Vector3.zero;
            reboundVelocity = Vector3.zero;
        }

        transform.position += (currentVelocity + reboundOffset)* Time.deltaTime ;
    }


    Vector3 InputDirection()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        return direction;
    }
}
