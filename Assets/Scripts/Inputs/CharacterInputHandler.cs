using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    PlayerModel _model;

    float _moveInput;
    bool _isJumpPressed;
    bool _isFirePressed;
    bool _isProtectPressed;

    void Start()
    {
        _model = GetComponent<PlayerModel>();
    }

    void Update()
    {
        if (!_model.HasInputAuthority) return;

        _moveInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.W))
        {
            _isJumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _isFirePressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isProtectPressed = true;
        }
    }

    //Metodo ejecutado por el Spawner, le provee todos los inputs del frame actual
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData inputData = new NetworkInputData();

        inputData.movementInput = _moveInput;

        inputData.isJumpPressed = _isJumpPressed;
        _isJumpPressed = false;

        inputData.isFirePressed = _isFirePressed;
        _isFirePressed = false;

        inputData.isProtectPressed = _isProtectPressed;
        _isProtectPressed = false;

        return inputData;
    }
}
