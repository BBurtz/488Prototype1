using UnityEngine;

public class Key : MonoBehaviour
{
    //key actually has to coincide with correct door...since they're in the same scene...
    //i cannot fucking code in class bro it's all fog
    private Door doorInstance;
    public GameObject correspondingDoor;

    private void Start()
    {
        doorInstance = correspondingDoor.GetComponent<Door>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")/*player when that happens*/ )
        {
            gameObject.SetActive(false);
            doorInstance.OpenDoor();
            Debug.Log("hi");
            
        }
    }
}
