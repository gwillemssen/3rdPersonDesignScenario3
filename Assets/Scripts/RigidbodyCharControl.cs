using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private readonly int movementInputAnimParam = Animator.StringToHash("movementInput");
    private Animator animator;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        Vector3 cameraRelativeInputDirection = GetCameraRelativeInputDirection();

        UpdatePhysicsMaterial();

        Move(cameraRelativeInputDirection);

        RotateToFaceInputDirection(cameraRelativeInputDirection);
    }

    /// <summary>
    /// Turning the character to face the direction it is moving based on player input
    /// </summary>
    /// <param name="movementDirection">the direction the character is trying to move.</param>
    private void RotateToFaceInputDirection(Vector3 movementDirection)
    {
        if (movementDirection.magnitude > 0)
        {
            var targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
        }
    }

    /// <summary>
    /// Moves the player in a direction based on its max speed and acceleration
    /// </summary>
    /// <param name="moveDirection">the direction to move in.</param>
    private void Move(Vector3 moveDirection)
    {
        if (rigidbody.velocity.magnitude < maxSpeed)
        {
            rigidbody.AddForce(moveDirection * accelerationForce, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// updates the physics material to a low friction option if the player is moving or a high friction option if they are trying to stop. 
    /// </summary>
    private void UpdatePhysicsMaterial()
    {
        collider.material = input.magnitude > 0 ? movingPhysicMaterial : stoppingPhysicMaterial;
    }

    /// <summary>
    /// Uses input vector to create a camera relative version so the player can move based on the cameras forward position
    /// </summary>
    /// <returns>returns the camera relative input direction.</returns>
    private Vector3 GetCameraRelativeInputDirection()
    {
        var inputDirection = new Vector3(input.x, 0, input.y);

        //below is a vector3
        var flatCameraForward = Camera.main.transform.forward;
        flatCameraForward.y = 0;
        var cameraRotation = Quaternion.LookRotation(flatCameraForward);

        Vector3 cameraRelativeInputDirectionReturn = cameraRotation * inputDirection;
        return cameraRelativeInputDirectionReturn;
    }

    /// <summary>
    /// This event handler is called from the player input component using the new input system
    /// </summary>
    /// <param name="context">vector 2 representing the move input.</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        animator.SetFloat(movementInputAnimParam, input.magnitude);
    }
}
