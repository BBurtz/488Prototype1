using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public GameObject Camera;
    public PlayerInput playerControls;
    public InputAction MoveAction;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        playerControls.ActivateInput();
        MoveAction = playerControls.currentActionMap.FindAction("Move");
        MoveAction.performed += move;
        /*playerControls.
        playerControls.Player.Pause.performed += Pause;
        playerControls.Player.Gun.performed += Gun;
        playerControls.Player.Jump.performed += Jump;*/
    }

    private void move(InputAction.CallbackContext context)
    {
        var c = context.ReadValue<Vector2>();
    }
}
