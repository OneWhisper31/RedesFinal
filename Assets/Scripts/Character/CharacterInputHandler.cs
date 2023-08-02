using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class CharacterInputHandler : MonoBehaviour
{
    float _horMove;
    float _verMove;
    bool _isJumpPressed;
    bool _isGrabPressed;

    NetworkInputData _inputData;

    private void Start()
    {
        _inputData = new NetworkInputData();
    }

    private void Update()
    {
        _horMove = Input.GetAxis("Horizontal");
        _verMove = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isJumpPressed = true;
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            _isGrabPressed = true;
        }
    }

    public NetworkInputData GetNetworkInput()
    {
        _inputData.horizontalInput = _horMove;
        _inputData.verticalInput = _verMove;

        _inputData.isJumpPressed = _isJumpPressed;
        _isJumpPressed = false;

        _inputData.isGrabPressed = _isGrabPressed;
        _isGrabPressed = false;

        return _inputData;
    }
}
