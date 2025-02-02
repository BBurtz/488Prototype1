using UnityEngine;
/*
 * Author: Sky Beal
 * Description: Sets corresponding door to "off".
 */
public class Door : MonoBehaviour
{
    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }
}
