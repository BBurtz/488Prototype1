/*****************************************************************************
// File Name :          BoxBehavior.cs
// Author :             Cade R. Naylor, Elda Osmani
// Creation Date :      January 29, 2025
// Modified Date :      January 29, 2025
// Last Modified By :   [NAME]
//
// Brief Description : Sets up and implements basic box behavior
                            - Box Gridded Movement
*****************************************************************************/
using System.Linq;
using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;

public class BoxBehavior : MonoBehaviour
{
    #region Variables
    [SerializeField, Range(.25f, 5), Tooltip("How wide the box is in Unity units")]
    private float boxWidth = 1f;        //Stores the size of the box. Used for calculations
    [SerializeField, Range(.5f, 3), Tooltip("The grid size, in Unity units. Should be at least half the box width.")]
    private float gridSize = .5f;       //Stores the size of the grid. Used for movement and calculations
    [SerializeField, Tooltip("The related box in the other dimension. Must be filled out on both boxes. If no linked object, leave blank.")]
    private GameObject linkedBox;       //Stores the linked box, should it have one

    private float moveTimer;            //An internal timer to track how long force has been applied
    private float forceTimeBeforeMove;  //The calculated value for how much time should elapse before the box moves
    
    /// <summary>
    /// Holds the different movement directions in a more readable way
    /// </summary>
    private enum forceDirection
    {
        POSX, NEGX, POSZ, NEGZ
    }

    #endregion

    #region Functions

    /// <summary>
    /// Called on the first frame update
    /// Handles variable initialization
    /// </summary>
    private void Start()
    {
        forceTimeBeforeMove = 1f * gridSize * boxWidth;
    }

    /// <summary>
    /// Called every frame this object has another object within its trigger
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        moveTimer += Time.deltaTime;
        if(moveTimer > forceTimeBeforeMove)
        {
            //Grab a reference to the player. Needs a check if the triggered object is the player
            GameObject player = other.gameObject;

            //Calculate the difference in position between this object and the next
            Vector2 difference = new Vector2(transform.position.x - player.transform.position.x, transform.position.z - player.transform.position.z);
            
            //Determine what direction the force is mainly coming from. Checks if x force > y force (using the absolute value to handle negatives)
            //Checks the polarity of the stronger coordinate and sets the force direction
            forceDirection forceDir = (Mathf.Abs(difference.x) > Mathf.Abs(difference.y) ? (difference.x > 0 ? forceDirection.POSX : forceDirection.NEGX) 
                : (difference.y > 0 ? forceDirection.POSZ : forceDirection.NEGZ));
            
            //Moves the box
            MoveBox(forceDir);

            //If this object has a link in the other world, call the linked box movement
            if(linkedBox != null)
            {
                //Move the linked box
                //NEEDS IMPLEMENTATION
                MoveLinkedBox(forceDir);
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
        //Set the modified position
        transform.position = modifiedPos;
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

    #endregion
}
