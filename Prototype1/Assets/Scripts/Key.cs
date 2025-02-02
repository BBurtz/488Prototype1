using UnityEngine;
/*
 * Author: Sky Beal
 * Description: On trigger enter, turns off corresponding door.
 */
public class Key : MonoBehaviour
{
    [Header ("Design")]
    [Tooltip ("Door the key turns off.")]
    public GameObject correspondingDoor;

    private Door doorInstance;

    private void Start()
    {
        doorInstance = correspondingDoor.GetComponent<Door>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (doorInstance != null)
        {
            if (other.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                doorInstance.OpenDoor();

            }
        }
    }

    /// <summary>
    /// Draws a line to the corresponding door.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (correspondingDoor == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gameObject.transform.position, correspondingDoor.transform.position);
        Gizmos.DrawWireMesh(correspondingDoor.GetComponent<MeshFilter>().sharedMesh, correspondingDoor.transform.position, correspondingDoor.transform.rotation, correspondingDoor.transform.lossyScale);
    }
}
