using UnityEngine;

public class Key : MonoBehaviour
{
    private Door doorInstance;
    public GameObject correspondingDoor;

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
}
