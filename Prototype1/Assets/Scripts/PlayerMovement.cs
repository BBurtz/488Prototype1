using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpStrength;

    public bool CurrentlyJumping;

    public GameObject Camera;
    public GameObject EndScrene;

    public PlayerInput playerControls;

    private InputAction MoveAction;
    private InputAction InteractAction;
    private InputAction SwitchAction;
    private InputAction DestroyAction;
    private InputAction JumpAction;

    [SerializeField, Tooltip("True if boxes move with pushing. False if 'E' is used to interact.")]
    private bool pushToMoveBlocks = false;
    [Tooltip("All boxes the player is currently in range of. All will move with 'E' if previous is False.")]
    public List<BoxBehavior> BoxesInRange = new List<BoxBehavior>();


    private Rigidbody rb;

    Vector2 MoveVal;

    Coroutine movementcoroutineInstance;

    private DimensionTransition dimensionTransition;
    [SerializeField] private BoxCreationDestruction boxCreationDestruction;

    public bool PushToMoveBlocks { get => pushToMoveBlocks;}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EndLine")
        {
            EndScrene.SetActive(true);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        dimensionTransition = FindObjectOfType<DimensionTransition>();
        boxCreationDestruction = FindObjectOfType<BoxCreationDestruction>();

    }

    private void OnEnable()
    {
        playerControls.ActivateInput();
        MoveAction = playerControls.currentActionMap.FindAction("Move");
        InteractAction = playerControls.currentActionMap.FindAction("Interact");
        SwitchAction = playerControls.currentActionMap.FindAction("Sprint");
        DestroyAction = playerControls.currentActionMap.FindAction("Destroy");
        JumpAction = playerControls.currentActionMap.FindAction("Jump");
        JumpAction.started += Jump;
        SwitchAction.started += shift;
        MoveAction.performed += move;
        MoveAction.canceled += stop;
        InteractAction.started += interact;
        DestroyAction.started += destroy;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!CurrentlyJumping)
        {
            rb.AddForce(0, jumpStrength, 0, ForceMode.Force);
            CurrentlyJumping = true;
            StartCoroutine(JumpReset());
        }
    }

    private void shift(InputAction.CallbackContext context)
    {
        dimensionTransition.SwapDimension();
    }

    /// <summary>
    /// Called when the interact key is pressed. 
    /// Calls MoveBox if interact is used to move box
    /// </summary>
    /// <param name="context"></param>
    private void interact(InputAction.CallbackContext context)
    {
        //Interaction with the pushblock code
        //Probably having this call a function in a different script would be the best
        foreach (BoxBehavior bb in BoxesInRange)
        {
            bb.CallMoveBox(gameObject);
        }
    }

    private void destroy(InputAction.CallbackContext context)
    {
        //Destroys boxes with the BoxCreationDestruction code
        boxCreationDestruction.destroylBox();
    }

    private void stop(InputAction.CallbackContext context)
    {
        StopCoroutine(movementcoroutineInstance);
        movementcoroutineInstance = null;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    private void move(InputAction.CallbackContext context)
    {
        MoveVal  = context.ReadValue<Vector2>();
        if(movementcoroutineInstance == null )
        {
            movementcoroutineInstance = StartCoroutine(Movement());
        }

    }

    private IEnumerator JumpReset()
    {
        yield return new WaitForSeconds(3);
        CurrentlyJumping = false;
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
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
