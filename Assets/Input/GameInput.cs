using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Scripts.Player;
using Game.Scripts.LiveObjects;

public class GameInput : MonoBehaviour
{

    private InputActions _input;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private Laptop _laptop;

    public static event Action<bool> _onInteractionInput;


    void OnEnable()
    {
        Laptop.onHackComplete += Laptop_onHackComplete;
        Laptop.onHackEnded += Laptop_onHackEnded;
    }



    void Start()
    {
        InitializeInput();
        SubscribeToActionMapActions();
    }

    private void InitializeInput()
    {
        _input = new InputActions();
        _input.Player.Enable();

    }

    private void SubscribeToActionMapActions()
    {
        //Player Actions
        _input.Player.Interact.performed += Interact_performed;
        _input.Player.Interact.canceled += Interact_canceled;

        //Laptop Actions
        _input.Laptop.SwapCameras.performed += SwapCameras_performed;
        _input.Laptop.ExitCameraMode.performed += ExitCameraMode_performed;
        
    }



    #region Laptop Actions
    private void Laptop_onHackComplete()
    {
        _input.Player.Disable();
        _input.Laptop.Enable();
    }
    private void Laptop_onHackEnded()
    {
        _input.Laptop.Disable();
        _input.Player.Enable();
    }

    private void SwapCameras_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _laptop.SwitchCameras();
    }
 
    private void ExitCameraMode_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _laptop.ExitCameraMode();
    }
    #endregion

    #region Player Actions
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _onInteractionInput.Invoke(true);
    }

    private void Interact_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _onInteractionInput.Invoke(false);
    }
    #endregion

    void Update()
    {
        //Player Movement Direction
        var direction = _input.Player.Movement.ReadValue<Vector2>();
        _player.GetMovementInput(direction);
    }
}
