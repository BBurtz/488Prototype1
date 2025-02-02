using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    public GameObject Camera;

    public PlayerInput playerControls;

    public InputAction MoveAction;
    public InputAction InteractAction;
    public InputAction SwitchAction;

    private Rigidbody rb;

    Vector2 MoveVal;

    Coroutine movementcoroutineInstance;

    private DimensionTransition dimensionTransition;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        dimensionTransition = FindObjectOfType<DimensionTransition>();


    }

    private void OnEnable()
    {
        playerControls.ActivateInput();
        MoveAction = playerControls.currentActionMap.FindAction("Move");
        InteractAction = playerControls.currentActionMap.FindAction("Interact");
        SwitchAction = playerControls.currentActionMap.FindAction("Sprint");
        SwitchAction.started += shift;
        MoveAction.performed += move;
        MoveAction.canceled += stop;
        InteractAction.started += interact;
    }

    private void shift(InputAction.CallbackContext context)
    {
        dimensionTransition.SwapDimension();
    }

    private void interact(InputAction.CallbackContext context)
    {
        //Interaction with the pushblock code
        //Probably having this call a function in a different script would be the best
    }

    private void stop(InputAction.CallbackContext context)
    {
        StopCoroutine(movementcoroutineInstance);
        movementcoroutineInstance = null;
        rb.linearVelocity = Vector3.zero;
    }

    private void move(InputAction.CallbackContext context)
    {
        MoveVal  = context.ReadValue<Vector2>();
        if(movementcoroutineInstance == null )
        {
            movementcoroutineInstance = StartCoroutine(Movement());
        }

    }

    public IEnumerator Movement()
    {
        while (true)
        {
            var c = MoveVal;
            Vector3 moveDirection = Camera.transform.forward * c.y + Camera.transform.right * c.x;
            moveDirection.y = 0;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, 0, limitedVel.z);
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
