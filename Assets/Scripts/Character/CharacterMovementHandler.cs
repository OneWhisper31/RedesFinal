using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

[RequireComponent(typeof(NetworkCharacterControllerPrototype))]
public class CharacterMovementHandler : NetworkBehaviour
{
    [SerializeField] BoxCollider pushManager;

    NetworkCharacterControllerPrototype _myCharacterController;

    //NetworkMecanimAnimator _myCharacterAnim;

    void Awake()
    {
        _myCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        //if (!Object.HasInputAuthority) return;

        if (GetInput(out NetworkInputData networkInput))
        {
            Vector3 moveDirection = new Vector3(networkInput.horizontalInput, 0f, networkInput.verticalInput);


            _myCharacterController.Move(moveDirection);



            //transform.position = new Vector3(transform.position.x, 0f, 0f);

            if (networkInput.isJumpPressed)
            {
                //_myCharacterController.Jump();
            }

            
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CharacterMovementHandler>();

        if (player != null && player != this)
        {
            pushManager.enabled = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<CharacterMovementHandler>();

        if (player != null && player != this)
        {
            pushManager.enabled= false;
        }
    }
}
