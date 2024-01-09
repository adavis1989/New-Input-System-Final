using Game.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputActions _input;
    [SerializeField] private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateInputes();
    }

    void InstantiateInputes()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Interact.started += Interact_started;
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var move = _input.Player.Movement.ReadValue<Vector2>();
        //_player.Movement(move);
    }
}
