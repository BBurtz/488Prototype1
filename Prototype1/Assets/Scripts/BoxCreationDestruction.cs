/*****************************************************************************
// File Name :          BoxCreationDestruction.cs
// Author :             Elda Osmani
// Creation Date :      February 2, 2025
// Modified Date :      February 2, 2025
// Last Modified By :   [NAME]
//
// Brief Description : Creates and destroys boxes in dimensions
*****************************************************************************/
using UnityEngine;

public class BoxCreationDestruction : MonoBehaviour
{
    //[SerializeField] private GameObject linkedBox;
    public  GameObject originalBox;
    //[SerializeField] private Material origM;
    //[SerializeField] private Material deactiveM;
    //[SerializeField] bool isActive;
    [Tooltip("Length reference for shifting.")]
    [SerializeField] private Collider floorCollider;

    [Tooltip("How large the overlap box checks for collisions when shifting.")]
    public Vector3 sizeOfCollisionScan;

    [Tooltip("Decides which way box moves, trying checking this if boxes are going the wrong way.")]
    [SerializeField] private bool inNormalDimension = true;

    [Tooltip("Layer mask that is ignored when looking for collisions.")]
    public LayerMask IgnoreWhenShifting;

    [Tooltip("Offset for the shifting raycast. DO NOT PUT ANYTHING FOR Z.")]
    public Vector3 BoxShiftingOffset;

    private Vector3 floorLength;
    private Vector3 floorWidthAcrossX;
    private Vector3 calculatedLocation;

    /// <summary>
    /// Getting math variables
    /// </summary>
    private void Start()
    {
        floorLength = floorCollider.bounds.size;
        floorWidthAcrossX = new Vector3(0, 0, (floorLength.z - 1) / 2);
    }

    /// <summary>
    /// Shifts box to other dimension via Q
    /// </summary>
    public void destroyBox()
    {

        //shiftSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        //if nothing collides with the box
        if (!isInBox())
        {
            originalBox.transform.position = calculatedLocation;
            inNormalDimension = !inNormalDimension;
            FindObjectOfType<PlayerMovement>().CDInRange.Remove(this);
        }

        //if something collides with the box
        else if (isInBox())
        {
            Debug.Log("shifting into wall...");
        }
    }
 

    private void OnTriggerEnter(Collider other)
    {
        //Check if the object has player movement
        if (other.GetComponent<PlayerMovement>() != null)
        {
                //Add the current box to the player's box list
                other.GetComponent<PlayerMovement>().CDInRange.Add(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            other.gameObject.GetComponent<PlayerMovement>().CDInRange.Remove(this);
            print("Should have removed");
        }
    }

    private Vector3 CalculateTransitionPoint()
    {
        if (inNormalDimension)
        {
            calculatedLocation = new Vector3(originalBox.transform.position.x + BoxShiftingOffset.x, originalBox.transform.position.y + BoxShiftingOffset.y, (originalBox.transform.position.z + (floorWidthAcrossX.z + 1) * -1));
        }

        else if (!inNormalDimension)
        {
            calculatedLocation = new Vector3(originalBox.transform.position.x + BoxShiftingOffset.x, originalBox.transform.position.y + BoxShiftingOffset.y, (originalBox.transform.position.z + floorWidthAcrossX.z + 1));
        }

        return calculatedLocation;
    }

    private bool isInBox()
    {
        CalculateTransitionPoint();

        Collider[] colliders = { };
        colliders = Physics.OverlapBox(calculatedLocation, sizeOfCollisionScan / 2, Quaternion.identity, ~IgnoreWhenShifting);

        //if no collision
        if (colliders.Length == 0)
        {
            return false;
        }
        //if collision
        else
        {
            return true;
        }
    }


    private void OnDrawGizmos()
    {
        if (originalBox.transform.position == null)
        {
            originalBox.transform.position = gameObject.transform.position;
        }

        CalculateTransitionPoint();

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(originalBox.transform.position + BoxShiftingOffset, calculatedLocation);

        if (isInBox())
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(calculatedLocation, sizeOfCollisionScan / 2);
    }
}
