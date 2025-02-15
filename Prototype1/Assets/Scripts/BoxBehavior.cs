/*****************************************************************************
// File Name :          BoxBehavior.cs
// Author :             Cade R. Naylor, Elda Osmani
// Creation Date :      January 29, 2025
// Modified Date :      February 3, 2025
// Last Modified By :   Cade Naylor
//
// Brief Description : Sets up and implements basic box behavior
                            - Box Gridded Movement
                            - Box Treadmill Movement
*****************************************************************************/
using System.Linq;
using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using static TreadmillBehavior;

public class BoxBehavior : MonoBehaviour
{
    #region Variables
    [Range(.25f, 5), Tooltip("How wide the box is in Unity units")]
    private float boxWidth = 1f;        //Stores the size of the box. Used for calculations
    [SerializeField, Range(.5f, 3), Tooltip("The grid size, in Unity units. Should be at least half the box width.")]
    private float gridSize = .5f;       //Stores the size of the grid. Used for movement and calculations
    [SerializeField, Tooltip("The material the box is made out of. Metal boxes cannot be destroyed.")]
    private boxMaterial boxType = boxMaterial.WOOD;
    [SerializeField, Tooltip("The related box in the other dimension. Must be filled out on both boxes. If no linked object, leave blank.")]
    private GameObject linkedBox;       //Stores the linked box, should it have one
    [SerializeField] private LayerMask layerMask;
    [SerializeField, Tooltip("How much buffer space each box is given in terms of collisions"), Range(0f,1f)]
    private float buffer;

    private float moveTimer;            //An internal timer to track how long force has been applied
    private float forceTimeBeforeMove;  //The calculated value for how much time should elapse before the box moves

    private bool isOnTreadmill = false;     //An internal bool used to check whether the box should be moving automatically or not
    private Coroutine treadmillMovementCoroutine;       //Storage for the treadmill coroutine
    private Coroutine linkedTreadmillMovementCoroutine;       //Storage for the treadmill coroutine

    /// <summary>
    /// Holds the different movement directions in a more readable way
    /// </summary>
    private enum forceDirection
    {
        POSX, NEGX, POSZ, NEGZ
    }

    /// <summary>
    /// Holds the different materials the box can be made of in a more readable way
    /// </summary>
    private enum boxMaterial
    {
        METAL, WOOD
    }

    #endregion

    #region Functions

    /// <summary>
    /// Called on the first frame update
    /// Handles variable initialization
    /// </summary>
    private void Start()
    {
        boxWidth = transform.localScale.x;
        forceTimeBeforeMove = 1f * gridSize * boxWidth;
    }

    /// <summary>
    /// Called every frame this object has another object within its trigger
    /// Only calls the timer if PushToMoveBlocks is enabled
    /// </summary>
    /// <param name="other">The object in the trigger</param>
    private void OnCollisionStay(Collision collision)
    {
        //Check if the object has player movement
        if(collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            if (boxType == boxMaterial.WOOD)
            {
                return;
            }

            if (OverlapCheck(linkedBox))
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                MoveLinkedBox();
            }
            //If it does, check if the player pushes to move blocks
           /* if (other.GetComponent<PlayerMovement>().BoxesMoveFreely)
            {
                //Debug.Log(transform.GetComponent<Rigidbody>().linearVelocity);
                MoveLinkedBox();
            }*/
        }
    }

    /// <summary>
    /// Called the first frame when an object enters this trigger
    /// Only calls the list if PushToMoveBlocks is disabled
    /// </summary>
    /// <param name="other">The object in the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        //Check if the object has player movement
        if(other.GetComponent<PlayerMovement>()!=null)
        {
            //If it does, check if the player uses a key to move blocks
            if(!other.GetComponent <PlayerMovement>().BoxesMoveFreely)
            {
                //Add the current box to the player's box list
                other.GetComponent<PlayerMovement>().BoxesInRange.Add(this);
            }
        }
    }


    
    /// <summary>
    /// Resets movement timer if the player exits the trigger
    /// </summary>
    /// <param name="other">The object exiting the trigger</param>
    private void OnTriggerExit(Collider other)
    {
        moveTimer = 0;
        if(other.GetComponent<PlayerMovement>()!=null && !other.gameObject.GetComponent<PlayerMovement>().BoxesMoveFreely)
        {
            other.gameObject.GetComponent<PlayerMovement>().BoxesInRange.Remove(this);
        }
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }



    private void MoveLinkedBox()
    {
        if (linkedBox == null)
        {
            return;
        }

        Vector3 basicVel = transform.GetComponent<Rigidbody>().linearVelocity;
        Vector3 linkedVel = new Vector3(-basicVel.x, linkedBox.GetComponent<Rigidbody>().linearVelocity.y, -basicVel.z);
        
        linkedVel = Vector3.ClampMagnitude(linkedVel, basicVel.magnitude);
        linkedBox.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        linkedBox.GetComponent<Rigidbody>().AddForce(linkedVel, ForceMode.Impulse);
    }

    /// <summary>
    /// Handles the box's current state in relation to the treadmill
    /// Starts or stops a coroutine, depending on whether it has entered or exited
    /// </summary>
    /// <param name="speed">The speed of the treadmill, as a float</param>
    /// <param name="treadmillDir">The direction of movement, as a treadmillDirection enum</param>
    public void HandleTreadmill(float speed, TreadmillBehavior.treadmillDirection treadmillDir)
    {
        //If the box is currently on a treadmill, stop its treadmill movement
        if (isOnTreadmill)
        {
            StopCoroutine(treadmillMovementCoroutine);
            if (linkedBox != null)
            {

                StopCoroutine(linkedTreadmillMovementCoroutine);
                linkedBox.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        //Otherwise, start its treadmill movement
        else
        {
            treadmillMovementCoroutine = StartCoroutine(HandleTreadmillMovement(speed, treadmillDir, gameObject));
            if(linkedBox!=null)
            {

                linkedTreadmillMovementCoroutine = StartCoroutine(HandleTreadmillMovement(speed, switchDir(treadmillDir), linkedBox));
            }

        }

        //I SO wanted to do that as a conditional operator. I should have. Hello, welcome to "Cade is Sleepy and has No
        // Filter" time. You get comments like these in addition to your regularly scheduled helpful comments.

        //Toggle the variable state
        isOnTreadmill = !isOnTreadmill;
    }
    private treadmillDirection switchDir(treadmillDirection originalForce)
    {
        switch (originalForce)
        {
            case treadmillDirection.POSX:
                return treadmillDirection.NEGX;
            case treadmillDirection.NEGX:
                return treadmillDirection.POSX;
            case treadmillDirection.POSZ:
                return treadmillDirection.NEGZ;
            case treadmillDirection.NEGZ:
                return treadmillDirection.POSZ;
            default:
                Debug.LogError("Invalid Movement Direction Detected!");
                return 0;

        }

    }

    private bool OverlapCheck(GameObject box)
    {
        Vector3 castDir = -1*this.GetComponent<Rigidbody>().linearVelocity;
        castDir.y = 0;
        castDir.z += (castDir.z < 0? -1  : 1) * boxWidth / 2.0f + buffer;
        castDir.x += (castDir.z < 0 ? -1 : 1) * boxWidth / 2.0f + buffer;


        RaycastHit rh;
        Debug.DrawRay(box.transform.position, castDir, Color.red);
        Physics.Raycast(box.transform.position, castDir, out rh, 1, layerMask);

        if(rh.distance <= boxWidth/2 + buffer)
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Handles the logic and actual movement of the box on a treadmill
    /// </summary>
    /// <param name="speed">The speed of the treadmill, as a float</param>
    /// <param name="treadDir">The direction of movement, as a treadmillDirection enum.</param>
    /// Don't ask why one variable name, but not the other, changed from the last function. I'm tired.
    /// <returns></returns>
    private IEnumerator HandleTreadmillMovement(float speed, TreadmillBehavior.treadmillDirection treadmillDir, GameObject box)
    {

        print("Called");
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

            Rigidbody rb = box.GetComponent<Rigidbody>();

            Vector3 moveDirection = rb.linearVelocity;
            moveDirection += treadmillVel;
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
            //Wow so much change

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > speed)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
            
            yield return null;
        }
    }

}

    #endregion

