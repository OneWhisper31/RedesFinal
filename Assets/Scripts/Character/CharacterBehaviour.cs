using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    
    CharacterController characterController;

    float moveSpeed = 5f;
    float jumpForce = 5f;
    float gravity = 9.8f;
    float verticalVelocity = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public void Move(float x, float y)
    {
        Vector3 moveDirection = new Vector3(x, 0f, y);
        moveDirection = moveDirection.normalized; // Normalize the direction to avoid faster diagonal movement
        moveDirection *= moveSpeed;

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = 0f; // Reset vertical velocity when grounded
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // Apply movement
        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
    }

}
