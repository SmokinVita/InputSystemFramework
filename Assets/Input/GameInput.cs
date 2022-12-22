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
    [SerializeField]
    private Drone _drone;

    public static event Action<bool> _onInteractionInput;


    void OnEnable()
    {
        Laptop.onHackComplete += Laptop_onHackComplete;
        Laptop.onHackEnded += Laptop_onHackEnded;

        Drone.OnEnterFlightMode += Drone_OnEnterFlightMode;
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

        //Drone Actions
        _input.Drone.ExitMode.performed += ExitMode_performed;
        
    }

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

    #region Drone Actions
    private void Drone_OnEnterFlightMode()
    {
        _input.Drone.Enable();
        _input.Player.Disable();
    }
    private void ExitMode_performed(InputAction.CallbackContext obj)
    {
        _drone.ExitDroneMode();
        _input.Player.Enable();
        _input.Drone.Disable();
    }

    #endregion


    void Update()
    {
        //Player Movement Direction
        var playerDirection = _input.Player.Movement.ReadValue<Vector2>();
        _player.GetMovementInput(playerDirection);

        //Drone Movement
        var droneDirection = _input.Drone._3Dmovement.ReadValue<Vector3>();
        var droneRotation = _input.Drone.Rotate.ReadValue<float>();
        _drone.MovementInput(droneDirection, droneRotation);
    }

    void OnDisable()
    {
        Laptop.onHackComplete -= Laptop_onHackComplete;
        Laptop.onHackEnded -= Laptop_onHackEnded;
        Drone.OnEnterFlightMode -= Drone_OnEnterFlightMode;
    }
}
