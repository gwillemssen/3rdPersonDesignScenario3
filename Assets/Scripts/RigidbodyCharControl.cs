using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCharControl : MonoBehaviour
{
    [SerializeField]
    private float accelerationForce = 10;

    [SerializeField]
    private float maxSpeed = 2;

    [SerializeField]
    [Tooltip("Player turn speed. 0 = no turning, 1 = instant snap turning")]
    [Range (0,1)]
    private float turnSpeed = 0.1f;

    [SerializeField]

    private PhysicMaterial stoppingPhysicMaterial, movingPhysicMaterial;

    private new Rigidbody rigidbody;
    private Vector2 input;
    private new Collider collider;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        var inputDirection = new Vector3(input.x, 0, input.y);

        //below is a vector3
        var flatCameraForward = Camera.main.transform.forward;
        flatCameraForward.y = 0;
        var cameraRotation = Quaternion.LookRotation(flatCameraForward);

        Vector3 cameraRelativeInputDirection = cameraRotation * inputDirection;

        collider.material = inputDirection.magnitude > 0 ? movingPhysicMaterial : stoppingPhysicMaterial;

        if (rigidbody.velocity.magnitude < maxSpeed)
        {
            rigidbody.AddForce(cameraRelativeInputDirection * accelerationForce, ForceMode.Acceleration);
        }

        if (cameraRelativeInputDirection.magnitude > 0)
        {
            var targetRotation = Quaternion.LookRotation(cameraRelativeInputDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
        }

    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }
}
