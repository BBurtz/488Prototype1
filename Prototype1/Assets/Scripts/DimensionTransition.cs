using UnityEngine;

public class DimensionTransition : MonoBehaviour
{
    [SerializeField] private Collider floorCollider;
    private Vector3 floorLength;
    private Transform playerPosition;
    private Vector3 calculatedLocation;
    private bool inNormalDimension = true;
    public bool MirrorAlongX = true;
    private Vector3 floorWidthAcrossX;
    private Vector3 floorWidthAcrossZ;
    private float swappingCollisionOffset;

    private void Start()
    {
        playerPosition = FindObjectOfType<PlayerMovement>().transform;
        floorLength = floorCollider.bounds.size;
        floorWidthAcrossX = new Vector3 (0, 0, (floorLength.z - 1) / 2);
        floorWidthAcrossZ = new Vector3 ((floorLength.x - 1) / 2, 0, 0);
    }

    public void SwapDimension()
    {
        playerPosition.position = CalculateTransitionPoint();
        inNormalDimension = !inNormalDimension;
        //check if you can switch
        //teleport player to other transform
    }

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

    private bool isInBox(Vector3 calculatedPosition)
    {
        return true;
    }

    private void OnDrawGizmos()
    {
        if (playerPosition == null)
        {
            playerPosition = FindObjectOfType<PlayerMovement>().transform;

        }
        CalculateTransitionPoint();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPosition.position, calculatedLocation);
        Gizmos.DrawWireSphere(calculatedLocation, 5);
    }
}
