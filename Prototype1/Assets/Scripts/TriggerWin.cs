using UnityEngine;

public class TriggerWin : MonoBehaviour
{
    [SerializeField] private GameObject winCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
            winCanvas.SetActive(true);
    }
}
