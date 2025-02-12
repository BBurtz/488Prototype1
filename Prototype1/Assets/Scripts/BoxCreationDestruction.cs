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
    [SerializeField] private GameObject linkedBox;
    public  GameObject originalBox;
    [SerializeField] private Material origM;
    [SerializeField] private Material deactiveM;
    [SerializeField] bool isActive;
    [SerializeField] private Collider floorCollider;
    [Tooltip("Mirrors Across X or Z Axis - true is X.")]
    public bool MirrorAlongX;
    [Tooltip("How large the overlap box checks for collisions when shifting.")]
    public Vector3 sizeOfCollisionScan;

    private Vector3 floorLength;
    private Vector3 floorWidthAcrossX;
    private bool inNormalDimension = true;
    private Vector3 calculatedLocation;

    private void Start()
    {
        floorLength = floorCollider.bounds.size;
        floorWidthAcrossX = new Vector3(0, 0, (floorLength.z - 1) / 2);
        //floorWidthAcrossZ = new Vector3((floorLength.x - 1) / 2, 0, 0);
    }

    public void destroyBox()
    {

        //shiftSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        //if nothing collides with the player
        if (!isInBox())
        {
            originalBox.transform.position = calculatedLocation;
            inNormalDimension = !inNormalDimension;
        }

        //if something collides with the player
        else if (isInBox())
        {
            //CannotShift();
        }
    }
 

    private void OnTriggerEnter(Collider other)
    {
        //Check if the object has player movement
        if (other.GetComponent<PlayerMovement>() != null && other.gameObject.GetComponent<PlayerMovement>().BoxesMoveFreely)
        {
                //Add the current box to the player's box list
                other.GetComponent<PlayerMovement>().CDInRange.Add(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null && !other.gameObject.GetComponent<PlayerMovement>().BoxesMoveFreely)
        {
            other.gameObject.GetComponent<PlayerMovement>().CDInRange.Remove(this);
        }
    }

    private Vector3 CalculateTransitionPoint()
    {
        if (inNormalDimension)
        {
            if (MirrorAlongX)
            {
                calculatedLocation = new Vector3(originalBox.transform.position.x, originalBox.transform.position.y, (originalBox.transform.position.z + (floorWidthAcrossX.z + 1) * -1));
            }

            else
            {
                //calculatedLocation = new Vector3((originalBox.transform.position.x + (floorWidthAcrossZ.x + 1) * -1), playerPosition.position.y, playerPosition.position.z);
            }
        }

        else if (!inNormalDimension)
        {
            if (MirrorAlongX)
            {
                calculatedLocation = new Vector3(originalBox.transform.position.x, originalBox.transform.position.y, (originalBox.transform.position.z + floorWidthAcrossX.z + 1));
            }

            else
            {
                //calculatedLocation = new Vector3((playerPosition.position.x + floorWidthAcrossZ.x + 1), playerPosition.position.y, playerPosition.position.z);
            }
        }

        return calculatedLocation;
    }

    private bool isInBox()
    {
        CalculateTransitionPoint();

        Collider[] colliders = { };
        colliders = Physics.OverlapBox(calculatedLocation, sizeOfCollisionScan / 2, Quaternion.identity);

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
        Gizmos.DrawLine(originalBox.transform.position, calculatedLocation);

        if (isInBox())
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(calculatedLocation, sizeOfCollisionScan / 2);
    }
}
