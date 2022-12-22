using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Player;
using Game.Scripts.LiveObjects;

public class GameInput : MonoBehaviour
{

    private InputActions _input;
    [SerializeField]
    private Player _player;

    private InteractableZone _interactableZone;
    public static event Action<bool> _onInteractionInput;
    public static event Action _onExitMode;


    private bool _isHoldingKey = false;



    //Check each mode for event to subscribe to and on entering mode swap action maps
    //Need one for Player to Drone and back to Player
    //Player to laptop and back
    //Player to Forklift and back.

    void OnEnable()
    {
        Laptop.onHackComplete += ReleasePlayerControl; ;
        Laptop.onHackEnded += ReturnPlayerControl; ;
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
        _input.Laptop.SwapCameras.canceled += SwapCameras_canceled;
        _input.Laptop.ExitCameraMode.performed += ExitCameraMode_performed;
    }

    #region Laptop Actions
    private void ReturnPlayerControl()
    {
        _input.Laptop.Disable();
        _input.Player.Disable();
        Debug.Log("Player is active");
    }

    private void ReleasePlayerControl()
    {
        _input.Player.Disable();
        _input.Laptop.Enable();
        Debug.Log("Laptop is active");
    }

    private void SwapCameras_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _onInteractionInput?.Invoke(true);
    }
    private void SwapCameras_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _onInteractionInput?.Invoke(false);
    }
    private void ExitCameraMode_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _onExitMode?.Invoke();
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
