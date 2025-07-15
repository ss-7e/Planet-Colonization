using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraControl : MonoBehaviour
{
    public float speed = 10f;
    public float smoothTime = 0.2f;

    public bool useRebound = false;
    public float reboundDistance = 0.5f;
    public float reboundReturnTime = 0.2f;

    public float minRotation = -60f;    
    public float maxRotation = 60f;
    public float rotationSpeed = 30f; 
    public float rotationAcceleration = 5f;

    public float scrollSpeed = 10f;    
    public float minHeight = 6f;
    public float maxHeight = 12f;

    public float mouseSensitivity = 100f;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 velocitySmooth = Vector3.zero;
    private float targetHeight = 0f;

    private Vector3 lastInputDir = Vector3.zero;
    private Vector3 reboundOffset = Vector3.zero;
    private Vector3 reboundVelocity = Vector3.zero;

    private float targetYaw = 0f;
    private float currentYaw = 0f;
    private float smoothYawVelocity = 0f;


    void Start()
    {
        targetHeight = transform.position.y;
        currentYaw = transform.parent.transform.eulerAngles.y;
        targetYaw = currentYaw; //keep inspector initial value
    }
    void Update()
    {
        HandleMovement();
        HandleRotation();
    }
    void HandleRotation()
    {
        if (Input.GetMouseButton(2))    // Hold middle mouse button to rotate
        {
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false;
            float deltaX = Input.GetAxis("Mouse X");
            targetYaw += deltaX * mouseSensitivity;
            targetYaw = Mathf.Clamp(targetYaw, minRotation, maxRotation);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref smoothYawVelocity, smoothTime);

        if (Mathf.Abs(currentYaw - targetYaw) > 0.01f)
        {
            Vector3 rotate = transform.eulerAngles;
            rotate.y = currentYaw;
            transform.eulerAngles = rotate;
        }

        // look down based on height
        if (transform.rotation.eulerAngles.x != transform.position.y + 25f) //hard coded rotation offset
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = transform.position.y + 25f;
            transform.rotation = Quaternion.Euler(rotation);
        }

    }

    void HandleMovement()
    {
        Vector3 inputDir = InputDirection();
        Vector3 targetVelocity = inputDir.normalized * speed;
        float scroll = -Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            targetHeight += scroll * scrollSpeed * Time.deltaTime * 100;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        }
        targetVelocity.y = targetHeight - transform.position.y;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocitySmooth, smoothTime);

        if (inputDir != Vector3.zero)
        {
            lastInputDir = inputDir.normalized;
        }
        if (inputDir == Vector3.zero && currentVelocity.magnitude < 0.3f && useRebound)
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

        Quaternion flatRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        Vector3 localMove = currentVelocity + reboundOffset;
        Vector3 worldMove = flatRotation * new Vector3(localMove.x, 0f, localMove.z);
        worldMove.y = currentVelocity.y;
        transform.position += worldMove * Time.deltaTime;
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
