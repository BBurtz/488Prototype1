/*****************************************************************************
// File Name :          TreadmillBehavior.cs
// Author :             Cade R. Naylor
// Creation Date :      February 2, 2025
// Modified Date :      February 3, 2025
// Last Modified By :   Cade Naylor
//
// Brief Description : Implements basic treadmill behavior
                        - Treadmill auto-adjusts size and applies texture
                        - Treadmill calls required movement functions
*****************************************************************************/
using NUnit.Framework;
using System.Net;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TreadmillBehavior : MonoBehaviour
{
    #region Variables

    [Header("Standard Treadmill Values")]
    [SerializeField, Tooltip("The direction the treadmill moves items in.")]
    private treadmillDirection treadmillDir;
    [SerializeField, Tooltip("Material for a treadmill. Will override any other materials.")]
    private Material treadmillMaterial;
    [UnityEngine.Range(0.5f, 10), Tooltip("How fast the treadmill moves.")]
    public float speed;

    /*[Header("Linked Treadmill Values")]
    [SerializeField, Tooltip("The related treadmill in the other dimension. Must be filled out on both treadmills. If no linked object, leave blank.")]
    GameObject linkedTreadmill;*/

    [HideInInspector]       //This is not visible in the inspector. It is used internally, but accessed by other scripts
    public bool directionIsFlipped = false;

    private bool hasTriggered;      //This is used to call enter and exit functions once for items with multiple colliders

    /// <summary>
    /// Holds the different movement directions in a more readable way
    /// </summary>
    public enum treadmillDirection
    {
        POSX, NEGX, POSZ, NEGZ
    }
    #endregion

    #region Functions
    /// <summary>
    /// Called on the first frame update.
    /// Initializes Treadmill state and swaps scale paramaters as needed.
    /// </summary>
    void Start()
    {
        //Initialize a variable to adjust scale if needed
        Vector3 scaleAdjustment = transform.localScale;

        //Swap the X and Z values if the treadmill needs it. This will be helpful for proper material application
        if (scaleAdjustment.x > scaleAdjustment.z)
        {
            float temp = scaleAdjustment.x;
            scaleAdjustment.x = scaleAdjustment.z;
            scaleAdjustment.z = temp;
        }

        //Set the scale to its adjusted values
        transform.localScale = scaleAdjustment;
        SetDirection();
    }

    /// <summary>
    /// Sets treadmill to the desired direction and size.
    /// It gets a little funky
    /// </summary>
    private void SetDirection()
    {
        //Initialize a variable to adjust rotation if needed
        Vector3 rotationAdjustment = transform.rotation.eulerAngles;

        //Look at the desired treadmill direction. Adjust rotation as needed for proper material application.
        switch (treadmillDir)
        {
            case treadmillDirection.POSZ:
                break;
            case treadmillDirection.NEGZ:
                rotationAdjustment.y += 180;
                break;
            case treadmillDirection.POSX:
                rotationAdjustment.y += 90;
                break;
            case treadmillDirection.NEGX:
                rotationAdjustment.y += 270;
                break;
            default:
                Debug.LogError("Error: Invalid treadmill direction detected. ");
                break;
        }

        //Set the rotation to its adjusted values
        transform.rotation = Quaternion.Euler(rotationAdjustment);

        //Apply the treadmill material with an arrow in the correct direction
        GetComponent<MeshRenderer>().material = treadmillMaterial;
    }

    /// <summary>
    /// Public facing function that handles setting a treadmill to be flipped.
    /// Calls SetDirection()
    /// </summary>
    public void FlipTreadmillDirection()
    {
        //Switch the boolean value
        directionIsFlipped = !directionIsFlipped;

        //Adjust the current treadmill direction to the opposite direction on the same axis
        if(treadmillDir == treadmillDirection.POSZ)
        {
            treadmillDir = treadmillDirection.NEGZ;
        }
        else if (treadmillDir == treadmillDirection.NEGZ)
        {
            treadmillDir = treadmillDirection.POSZ;
        }
        else if (treadmillDir == treadmillDirection.POSX)
        {
            treadmillDir = treadmillDirection.NEGX;
        }
        else
        {
            treadmillDir = treadmillDirection.POSX;
        }

        //Adjust the visuals to match
        SetDirection();
    }

    /// <summary>
    /// Calls proper movement functions when objects move onto the treadmill
    /// </summary>
    /// <param name="other">The collider entering the Treadmill's space</param>
    private void OnTriggerEnter(Collider other)
    {
        //If the other object is a box AND it hasn't triggered this interaction
        if (other.GetComponent<BoxBehavior>() != null && !hasTriggered)
        {
            //Call the box's movement function
            other.GetComponent<BoxBehavior>().HandleTreadmill(speed, treadmillDir);
            hasTriggered = true;
            
        }
        //Otherwise if the other object is the player
        else if (other.GetComponent<PlayerMovement>()!=null)
        {
            //Call the player's movement function
            other.GetComponent<PlayerMovement>().HandleTreadmill(speed, treadmillDir);
        }

        //This code would have looked so much cleaner if PlayerMovement and BoxBehavior inherited from the same parent
        //It still would get a little messy, but not to this extent. 
        //However, it wouldn't have been worth going in and adding a parent class for this sole reason
    }

    /// <summary>
    /// Calls proper movement functions when objects leave the treadmill
    /// </summary>
    /// <param name="other">The collider exiting the Treadmill's space</param>
    private void OnTriggerExit(Collider other)
    {
        /*If the other object is a box AND it hasn't triggered this interaction
            It looks weird having it check if 'hasTriggered' is true, but it is only true if an object has entered.
            Thus, checking if it is true allows it to be set to false and reset easily.
            Is there bug potential if you have multiple boxes at once? Absolutely*/
        if (other.GetComponent<BoxBehavior>() != null && hasTriggered)
        {
            //Call the box's movement function
            other.GetComponent<BoxBehavior>().HandleTreadmill(speed, treadmillDir);
            hasTriggered = false;
        }
        //Otherwise if the other object is the player
        else if (other.GetComponent<PlayerMovement>() != null)
        {
            //Call the player's movement function
            other.GetComponent<PlayerMovement>().HandleTreadmill(speed, treadmillDir);

        }
    }
    #endregion
}
