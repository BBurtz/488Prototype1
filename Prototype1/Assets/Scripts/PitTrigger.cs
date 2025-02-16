using UnityEngine;

public class PitTrigger : MonoBehaviour
{
    [SerializeField] private Transform teleportPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement p) == true)
        {
            other.gameObject.transform.position = teleportPos.position;
        }
    }
}
