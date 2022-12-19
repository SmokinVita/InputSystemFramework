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
    [SerializeField]
    private InteractableZone _interactableZone;


    //Check each mode for event to subscribe to and on entering mode swap action maps
    //Need one for Player to Drone and back to Player
    //Player to laptop and back
    //Player to Forklift and back.


    void Start()
    {
        InitializeInput();
    }

    private void InitializeInput()
    {
        _input = new InputActions();
        _input.Player.Enable();
    }


    void Update()
    {
        var direction = _input.Player.Movement.ReadValue<Vector2>();
        _player.GetMovementInput(direction);
    }
}
