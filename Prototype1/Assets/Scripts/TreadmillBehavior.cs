/*****************************************************************************
// File Name :          TreadmillBehavior.cs
// Author :             Cade R. Naylor
// Creation Date :      February 2, 2025
// Modified Date :      February 2, 2025
// Last Modified By :   Cade Naylor
//
// Brief Description : Implements basic treadmill behavior
                        - Objects move in the direction the arrow points
*****************************************************************************/
using System.Net;
using UnityEngine;

public class TreadmillBehavior : MonoBehaviour
{
    #region Variables
    [Header("Standard Treadmill Values")]
    [SerializeField, Tooltip("The direction the treadmill moves items in.")]
    private treadmillDirection treadmillDir;
    [SerializeField, Tooltip("Material for a treadmill. Will override any other materials.")]
    private Material treadmillMaterial;
    [SerializeField, Range(0.5f, 10), Tooltip("How fast the treadmill moves.")]
    private float speed;

    /*[Header("Linked Treadmill Values")]
    [SerializeField, Tooltip("The related treadmill in the other dimension. Must be filled out on both treadmills. If no linked object, leave blank.")]
    GameObject linkedTreadmill;*/

    [HideInInspector]
    public bool directionIsFlipped = false;

    private enum treadmillDirection
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

    private void OnTriggerStay(Collider other)
    {

    }


    #endregion
}
