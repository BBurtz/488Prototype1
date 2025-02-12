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

public class OldBoxBehavior : MonoBehaviour
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

    private float moveTimer;            //An internal timer to track how long force has been applied
    private float forceTimeBeforeMove;  //The calculated value for how much time should elapse before the box moves

    private bool isOnTreadmill = false;     //An internal bool used to check whether the box should be moving automatically or not
    private Coroutine treadmillMovementCoroutine;       //Storage for the treadmill coroutine

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
    private void OnTriggerStay(Collider other)
    {
        //Check if the object has player movement
        if (other.GetComponent<PlayerMovement>() != null)
        {
            //If it does, check if the player pushes to move blocks
            if (other.GetComponent<PlayerMovement>().BoxesMoveFreely)
            {
                //Increase the timer if they do
                moveTimer += Time.deltaTime;

                //If the timer is full, call the box push
                if (moveTimer > forceTimeBeforeMove)
                {
                    CallMoveBox(other.gameObject);
                }
            }
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
        if (other.GetComponent<PlayerMovement>() != null)
        {
            //If it does, check if the player uses a key to move blocks
            if (!other.GetComponent<PlayerMovement>().BoxesMoveFreely)
            {
                //Add the current box to the player's box list
                //other.GetComponent<PlayerMovement>().BoxesInRange.Add(this);
            }
        }
    }

    /// <summary>
    /// A general function that handles box pushing
    /// Gets the push direction and moves the box in the appropriate direction
    /// Handles calls to linked boxes
    /// </summary>
    /// <param name="player">A reference to the player object</param>
    public void CallMoveBox(GameObject player)
    {
        //Calculate the difference in position between this object and the next
        Vector2 difference = new Vector2(transform.position.x - player.transform.position.x, transform.position.z - player.transform.position.z);

        //Determine what direction the force is mainly coming from. Checks if x force > y force (using the absolute value to handle negatives)
        //Checks the polarity of the stronger coordinate and sets the force direction
        forceDirection forceDir = (Mathf.Abs(difference.x) > Mathf.Abs(difference.y) ? (difference.x > 0 ? forceDirection.POSX : forceDirection.NEGX)
            : (difference.y > 0 ? forceDirection.POSZ : forceDirection.NEGZ));

        //Moves the box
        MoveBox(forceDir);

    }


    /// <summary>
    /// Resets movement timer if the player exits the trigger
    /// </summary>
    /// <param name="other">The object exiting the trigger</param>
    private void OnTriggerExit(Collider other)
    {
        moveTimer = 0;
        if (other.GetComponent<PlayerMovement>() != null && !other.gameObject.GetComponent<PlayerMovement>().BoxesMoveFreely)
        {
            //other.gameObject.GetComponent<PlayerMovement>().BoxesInRange.Remove(this);
        }
    }

    /// <summary>
    /// Handles box movement in a grid. Throws an error if invalid movement is detected. 
    /// </summary>
    /// <param name="forceDir">The direction force is being applied from</param>
    private void MoveBox(forceDirection forceDir)
    {
        moveTimer = 0;
        Vector3 modifiedPos = transform.position;
        //You will note in all cases it applies force in the opposite direction from the applied from. 
        //If force is applied from positive x, the box should move in a negative x, and so on and so forth

        //Checks the movement direction and adjusts the corresponding value of modifiedPos.
        //Throws an error if there is an invalid movement type
        switch (forceDir)
        {
            case forceDirection.POSX:
                modifiedPos.x += gridSize;
                break;
            case forceDirection.NEGX:
                modifiedPos.x -= gridSize;
                break;
            case forceDirection.POSZ:
                modifiedPos.z += gridSize;
                break;
            case forceDirection.NEGZ:
                modifiedPos.z -= gridSize;
                break;
            default:
                Debug.LogError("Invalid Movement Direction Detected!");
                break;

        }
        Debug.Log(gameObject.name + ": " + forceDir);
        //Set the modified position
        bool theCheck = OverlapCheck(this.gameObject, forceDir);
        if (theCheck == false)
        {
            //If this object has a link in the other world, call the linked box movement
            if (linkedBox != null)
            {
                forceDirection temp = switchDir(forceDir);
                if (!OverlapCheck(linkedBox, temp))
                {
                    //Move the linked box
                    transform.position = modifiedPos;
                    MoveLinkedBox(forceDir);
                }
            }
            else
            {
                transform.position = modifiedPos;
            }
        }
        else
        {
            Debug.Log("Cannot move");
        }
    }

    /// <summary>
    /// Handles mirrored box movement in a grid. Throws an error if invalid movement is detected
    /// </summary>
    /// <param name="forceDir"></param>
    private void MoveLinkedBox(forceDirection forceDir)
    {
        //Pretty much copy the same as above, but flip the signs
        //Change the transform.position to that of the linked box
        //It should work and create parity. However, no checks are in place

        moveTimer = 0;
        Vector3 linkedModifiedPos = linkedBox.transform.position;

        switch (forceDir)
        {
            case forceDirection.POSX:
                linkedModifiedPos.x -= gridSize;
                break;
            case forceDirection.NEGX:
                linkedModifiedPos.x += gridSize;
                break;
            case forceDirection.POSZ:
                linkedModifiedPos.z -= gridSize;
                break;
            case forceDirection.NEGZ:
                linkedModifiedPos.z += gridSize;
                break;
            default:
                Debug.LogError("Invalid Movement Direction Detected!");
                break;

        }
        //Set the modified position
        linkedBox.transform.position = linkedModifiedPos;

    }

    private forceDirection switchDir(forceDirection originalForce)
    {
        switch (originalForce)
        {
            case forceDirection.POSX:
                return forceDirection.NEGX;
            case forceDirection.NEGX:
                return forceDirection.POSX;
            case forceDirection.POSZ:
                return forceDirection.NEGZ;
            case forceDirection.NEGZ:
                return forceDirection.POSZ;
            default:
                Debug.LogError("Invalid Movement Direction Detected!");
                return 0;

        }

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
        }
        //Otherwise, start its treadmill movement
        else
        {
            treadmillMovementCoroutine = StartCoroutine(HandleTreadmillMovement(speed, treadmillDir));
        }

        //I SO wanted to do that as a conditional operator. I should have. Hello, welcome to "Cade is Sleepy and has No
        // Filter" time. You get comments like these in addition to your regularly scheduled helpful comments.

        //Toggle the variable state
        isOnTreadmill = !isOnTreadmill;
    }

    private bool OverlapCheck(GameObject box, forceDirection forceDir)
    {
        Vector3 castDir = Vector3.zero;

        switch (forceDir)
        {
            case forceDirection.POSX:
                castDir.x += gridSize;
                break;
            case forceDirection.NEGX:
                castDir.x -= gridSize;
                break;
            case forceDirection.POSZ:
                castDir.z += gridSize;
                break;
            case forceDirection.NEGZ:
                castDir.z -= gridSize;
                break;
            default:
                Debug.LogError("Invalid Movement Direction Detected!");
                break;

        }

        RaycastHit rh;
        Debug.DrawRay(box.transform.position, castDir, Color.red);
        bool hit = Physics.Raycast(box.transform.position, castDir * 1, out rh, 1, layerMask);

        return hit;
    }


    /// <summary>
    /// Handles the logic and actual movement of the box on a treadmill
    /// </summary>
    /// <param name="speed">The speed of the treadmill, as a float</param>
    /// <param name="treadDir">The direction of movement, as a treadmillDirection enum.</param>
    /// Don't ask why one variable name, but not the other, changed from the last function. I'm tired.
    /// <returns></returns>
    private IEnumerator HandleTreadmillMovement(float speed, TreadmillBehavior.treadmillDirection treadDir)
    {
        //Sets up an infinite loop, but safely
        while (true)
        {
            //Yeah, I copied previous movement code for this. How can you tell?
            //Declares and initializes a variable to hold modified position
            Vector3 modifiedPos = transform.position;

            //Checks the movement direction and adjusts the corresponding value of modifiedPos.
            //Throws an error if there is an invalid movement type
            switch (treadDir)
            {
                case TreadmillBehavior.treadmillDirection.POSX:
                    modifiedPos.x += gridSize;
                    break;
                case TreadmillBehavior.treadmillDirection.NEGX:
                    modifiedPos.x -= gridSize;
                    break;
                case TreadmillBehavior.treadmillDirection.POSZ:
                    modifiedPos.z += gridSize;
                    break;
                case TreadmillBehavior.treadmillDirection.NEGZ:
                    modifiedPos.z -= gridSize;
                    break;
                default:
                    Debug.LogError("Invalid Treadmill Direction Detected!");
                    break;

            }

            //Set the modified position
            transform.position = modifiedPos;

            //Wait 1/speed seconds. This way, faster speeds move faster. Slower speeds move slower.
            yield return new WaitForSeconds(1.0f / speed);
        }
    }

    #endregion
}
