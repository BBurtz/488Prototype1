using UnityEngine;
/*
 * Author: Sky Beal
 * Description: Swaps player between two dimensions. Runs collision checks and draws gizmos for debugging.
 */

public class DimensionTransition : MonoBehaviour
{
    [Header ("Design")]

    [Tooltip ("Mirrors Across X or Z Axis - true is X.")]
    public bool MirrorAlongX;

    [Tooltip("How far and which direction the player is sent if shifting into an object.")]
    public Vector3 swappingCollisionOffset;

    [Tooltip("How large the overlap box checks for collisions when shifting.")]
    public Vector3 sizeOfCollisionScan;



    [Header ("Calculations")]

    [Tooltip("Floor collider, calculates the length of the room to shift appropriately.")]
    [SerializeField] private Collider floorCollider;

    [Tooltip("WALLS AND FLOOR SHOULD BE ON THIS LAYER--Layer mask that is ignored when looking for collisions.")]
    public LayerMask IgnoreWhenShifting;

    //length of the floor
    private Vector3 floorLength;
    //current player position
    private Transform playerPosition;
    //location where player will shift to
    private Vector3 calculatedLocation;
    //if player is in normal or alternate dimension
    private bool inNormalDimension = true;
    //for calculating mirroring across x axis
    private Vector3 floorWidthAcrossX;
    //for calculating mirroring across z axis
    private Vector3 floorWidthAcrossZ;
    

    private void Start()
    {
        playerPosition = FindObjectOfType<PlayerMovement>().transform;
        floorLength = floorCollider.bounds.size;
        floorWidthAcrossX = new Vector3 (0, 0, (floorLength.z - 1) / 2);
        floorWidthAcrossZ = new Vector3 ((floorLength.x - 1) / 2, 0, 0);
    }

    /// <summary>
    /// Called from PlayerMovement action to swap dimensions
    /// </summary>
    public void SwapDimension()
    {
        //if nothing collides with the player
        if (!isInBox())
        {
            playerPosition.position = calculatedLocation;
            inNormalDimension = !inNormalDimension;
        }
        //if something collides with the player
        else if (isInBox())
        {
            playerPosition.position = calculatedLocation + swappingCollisionOffset;
            inNormalDimension = !inNormalDimension;
        }
    }

    /// <summary>
    /// Calculates the relative point in the other dimension that the player should shift to.
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateTransitionPoint()
    {

        if (inNormalDimension)
        {
            if (MirrorAlongX)
            {
                calculatedLocation = new Vector3 (playerPosition.position.x, playerPosition.position.y, (playerPosition.position.z + (floorWidthAcrossX.z + 1)*-1));
            }

            else
            {
                calculatedLocation = new Vector3((playerPosition.position.x + (floorWidthAcrossZ.x + 1)*-1), playerPosition.position.y, playerPosition.position.z);
            }
        }

        else if (!inNormalDimension)
        {
            if (MirrorAlongX)
            {
                calculatedLocation = new Vector3(playerPosition.position.x, playerPosition.position.y, (playerPosition.position.z + floorWidthAcrossX.z + 1));
            }

            else
            {
                calculatedLocation = new Vector3((playerPosition.position.x + floorWidthAcrossZ.x + 1), playerPosition.position.y, playerPosition.position.z);
            }
        }

        return calculatedLocation;
    }

    /// <summary>
    /// Runs an overlap box to see if the player is shifting into an object.
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// Draws expected location the player will shift to. Turns red if player is shifting into an object.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (playerPosition == null)
        {
            playerPosition = FindObjectOfType<PlayerMovement>().transform;
        }

        CalculateTransitionPoint();

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerPosition.position, calculatedLocation);

        if (isInBox())
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(calculatedLocation, sizeOfCollisionScan / 2);
    }
}
