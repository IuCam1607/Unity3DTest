using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementTest : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpButtonGracePeriod;

    private float horizontalInput;
    private float verticalInput;
    private float yForce;

    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressTime;

    private CharacterController characterController;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3 (horizontalInput, 0, verticalInput);

        float magnitude = movementDirection.magnitude;
        magnitude = Mathf.Clamp01(magnitude);

        movementDirection.Normalize();        //chuẩn hóa Vector

        yForce += Physics.gravity.y * Time.deltaTime;

        if(characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if(Input.GetButtonDown("Jump"))
        {
            jumpButtonPressTime = Time.time;
        }    

        if(Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            yForce = -0.5f;
            characterController.stepOffset = originalStepOffset;
            if (Time.time - jumpButtonPressTime <= jumpButtonGracePeriod)
            {
                yForce = jumpForce;
                jumpButtonPressTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = moveSpeed * magnitude * movementDirection;
        velocity.y = yForce;

        characterController.Move(velocity * Time.deltaTime);

        if(movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);// xoay hướng nhân vật
        }
    }
}
