/*****************************************************************************
// File Name :          PlayerMovement.cs
// Author :             Brenden Burtz
// Creation Date :      January 29, 2025
// Modified Date :      February 3, 2025
// Last Modified By :   Cade Naylor
//
// Brief Description :  Handles player input controls
                            - Player Movement, both normal and treadmill
                            - Calls interaction functions
*****************************************************************************/
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using FMODUnity;
using FMOD.Studio;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpStrength;

    public bool CurrentlyJumping;
    private bool CurrentlyMoving;

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

    private bool movementOverrideForTreadmill = false;      //A boolean storing whether the movement should be paused for treadmill movement
    private Coroutine treadmillMovementCoroutine;       //Stores the treadmill movement coroutine while moving on it

    private DimensionTransition dimensionTransition;
    [SerializeField] private BoxCreationDestruction boxCreationDestruction;

    private EventInstance walkSFX;
    private EventInstance jumpSFX;
    

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

        //audio
        walkSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.Walk);
        jumpSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.Jump);
    }
    private void Update()
    {
        //update sound location to stay on player
        walkSFX.set3DAttributes(RuntimeUtils.To3DAttributes(GetComponent<Transform>(), GetComponent<Rigidbody>()));
        jumpSFX.set3DAttributes(RuntimeUtils.To3DAttributes(GetComponent<Transform>(), GetComponent<Rigidbody>()));
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

    private void FixedUpdate()
    {
        if (!movementOverrideForTreadmill)
        {
            if (!CurrentlyMoving)
            {
                MoveVal = Vector3.zero;
            }
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
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!CurrentlyJumping)
        {
            jumpSFX.start();
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
        boxCreationDestruction.destroyBox();
    }

    private void stop(InputAction.CallbackContext context)
    {
        /*StopCoroutine(movementcoroutineInstance);
        movementcoroutineInstance = null;*/
        CurrentlyMoving = false;
        MoveVal = new Vector3(0, rb.linearVelocity.y, 0);
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    private void move(InputAction.CallbackContext context)
    {
        CurrentlyMoving = true;
        MoveVal  = context.ReadValue<Vector2>();
        /*if(movementcoroutineInstance == null )
        {
            movementcoroutineInstance = StartCoroutine(Movement());
        }*/

    }

    private IEnumerator JumpReset()
    {
        yield return new WaitForSeconds(1.5f);
        CurrentlyJumping = false;
    }

    /// <summary>
    /// Coroutine for movement under normal conditions
    /// </summary>
    /// <returns>Time waited between calls</returns>
    /*public IEnumerator Movement()
    {
        while (true)
        {
            if(!movementOverrideForTreadmill)
            {
                var c = MoveVal;
                Vector3 moveDirection = Camera.transform.forward * c.y + Camera.transform.right * c.x;
                moveDirection.y = 0;
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * Time.deltaTime, ForceMode.Force);

                Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                if (flatVel.magnitude > moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
                }
                UpdateWalkSFX();
            }
            yield return null;
        }
    }*/

    /// <summary>
    /// Handles the player's current state in relation to the treadmill
    /// Starts or stops a coroutine, depending on whether it has entered or exited
    /// </summary>
    /// <param name="speed">The speed of the treadmill, as a float</param>
    /// <param name="treadmillDir">The direction of movement, as a treadmillDirection enum</param>
    public void HandleTreadmill(float speed, TreadmillBehavior.treadmillDirection treadmillDir)
    {
        //If the box is currently on a treadmill, stop its treadmill movement
        if (movementOverrideForTreadmill)
        {
            StopCoroutine(treadmillMovementCoroutine);

            //Stop any lingering velocity for consistency
            rb.linearVelocity = Vector3.zero;
        }
        //Otherwise, start its treadmill movement
        else
        {
            treadmillMovementCoroutine = StartCoroutine(HandleTreadmillMovement(speed, treadmillDir));
        }

        //I'll spare the snarky comment here. See lines 246/247 on BoxBehavior

        //Toggle the variable state
        movementOverrideForTreadmill = !movementOverrideForTreadmill;
        
    }

    /// <summary>
    /// Handles the logic and actual movement of the box on a treadmill
    /// </summary>
    /// <param name="speed">The speed of the treadmill, as a float</param>
    /// <param name="treadDir">The direction of movement, as a treadmillDirection enum.</param>
    /// Don't ask why one variable name, but not the other, changed from the last function. I'm tired.
    /// <returns></returns>
    private IEnumerator HandleTreadmillMovement(float speed, TreadmillBehavior.treadmillDirection treadmillDir)
    {
        //Sets up an infinite loop, but safely
        while (true)
        {
            //Declares and initializes a variable to hold the velocity direction
            Vector3 treadmillVel = Vector3.zero;

            //Checks the treadmill direction and adjusts the corresponding value of treadmillVel.

            if (treadmillDir == TreadmillBehavior.treadmillDirection.POSZ)
            {
                treadmillVel.z += speed;
            }
            else if (treadmillDir == TreadmillBehavior.treadmillDirection.NEGZ)
            {
                treadmillVel.z -= speed;
            }
            else if (treadmillDir == TreadmillBehavior.treadmillDirection.POSX)
            {
                treadmillVel.x += speed;
            }
            else
            {
                treadmillVel.x -= speed;
            }

            //Wowwie this code is fully original
            //It's not the same movement code as Brenden wrote above with a couple small tweaks
            //Nnnope 
            //I swear I still have marbles
            var c = MoveVal;
            Vector3 moveDirection = Camera.transform.forward * c.y + Camera.transform.right * c.x + treadmillVel;
            moveDirection.y = 0;
            rb.AddForce(moveDirection.normalized * moveSpeed * speed * 10f, ForceMode.Force);
            //Wow so much change

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }

            yield return null;
        }
    }
    private void UpdateWalkSFX()
    {
        if ((rb.linearVelocity.x > 0 || rb.linearVelocity.z > 0) && rb.linearVelocity.y < 0.01 )
        {
            PLAYBACK_STATE playbackState;
            walkSFX.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                walkSFX.start();
            }
        }
        else
        {
            walkSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

}
