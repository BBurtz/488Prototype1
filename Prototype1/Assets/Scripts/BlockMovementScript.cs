using UnityEngine;

public class BlockMovementScript : MonoBehaviour
{
    [SerializeField] GameObject blockBox;
    [SerializeField] GameObject linkedBlockBox;
    public bool boxBlocked = false;
    public bool linkedBoxBlocked = false;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("RealWall"))
        {
            boxBlocked = true;
        }
        else if (other.CompareTag("ImaginaryWall"))
        {
            linkedBoxBlocked = true;
        }
    }

}
