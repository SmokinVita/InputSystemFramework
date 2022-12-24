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
    [SerializeField]
    private Forklift _forkLift;
    [SerializeField]
    private Crate _crate;

    public static event Action<bool> _onInteractionInput;


    void OnEnable()
    {
        //Subscribing to the Scenarios 
        Laptop.onHackComplete += Laptop_onHackComplete;
        Laptop.onHackEnded += Laptop_onHackEnded;
        Drone.OnEnterFlightMode += Drone_OnEnterFlightMode;
        Forklift.onDriveModeEntered += Forklift_onDriveModeEntered;
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
        _input.Drone.ExitMode.performed += Drone_Exit;

        //Forklift Actions
        _input.Forklift.ExitMode.performed += Forklift_Exit;


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
    private void Drone_Exit(InputAction.CallbackContext obj)
    {
        _drone.ExitDroneMode();
        _input.Player.Enable();
        _input.Drone.Disable();
    }

    #endregion

    #region Forklift Actions
    private void Forklift_Exit(InputAction.CallbackContext obj)
    {
        _input.Forklift.Disable();
        _input.Player.Enable();
    }
    private void Forklift_onDriveModeEntered()
    {
        _input.Player.Disable();
        _input.Forklift.Enable();
    }
    #endregion

    void Update()
    {
        if (_input.Player.enabled == true)
        {
            //Player Movement Direction
            var playerDirection = _input.Player.Movement.ReadValue<Vector2>();
            _player.GetMovementInput(playerDirection);
        }
        else if (_input.Drone.enabled == true)
        {
            //Drone Movement
            var droneDirection = _input.Drone._3Dmovement.ReadValue<Vector3>();
            var droneRotation = _input.Drone.Rotate.ReadValue<float>();
            _drone.MovementInput(droneDirection, droneRotation);
        }
        else if (_input.Forklift.enabled == true)
        {
            //Forklift
            var forkliftDirection = _input.Forklift.Movement.ReadValue<Vector2>();
            var liftInput = _input.Forklift.RiseandDropLift.ReadValue<float>();
            _forkLift.GetInput(forkliftDirection, liftInput);
        }
    }

    IEnumerator HoldForBreakRoutine()
    {
        while(true)
        {
            _crate.BreakPart();
            yield return new WaitForSeconds(1f);
        }
    }

    void OnDisable()
    {
        Laptop.onHackComplete -= Laptop_onHackComplete;
        Laptop.onHackEnded -= Laptop_onHackEnded;
        Drone.OnEnterFlightMode -= Drone_OnEnterFlightMode;
    }
}
