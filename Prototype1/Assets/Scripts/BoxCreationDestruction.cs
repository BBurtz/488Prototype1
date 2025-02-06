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

    /*
    public void destroyBox()
    {
        if (originalBox.activeInHierarchy && !linkedBox.activeInHierarchy)
        {
            linkedBox.SetActive(true);
            originalBox.SetActive(false);
            Debug.Log("Does destroying original work?");
        }
        else if (!originalBox.activeInHierarchy && linkedBox.activeInHierarchy)
        {
            originalBox.SetActive(true);
            linkedBox.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Warning! Make sure one of the boxes is inactive in the hierarchy");
        }
    }
    */

    public void destroyBox()
    {
        if(originalBox.GetComponent<MeshRenderer>().material == origM)
        {
            originalBox.GetComponent<MeshRenderer>().material = deactiveM;
            linkedBox.GetComponent<MeshRenderer>().material = origM;
        }
        else
        {
            linkedBox.GetComponent<MeshRenderer>().material = deactiveM;
            originalBox.GetComponent<MeshRenderer>().material = origM;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if the object has player movement
        if (other.GetComponent<PlayerMovement>() != null)
        {
                //Add the current box to the player's box list
                other.GetComponent<PlayerMovement>().CDInRange.Add(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null && !other.gameObject.GetComponent<PlayerMovement>().PushToMoveBlocks)
        {
            other.gameObject.GetComponent<PlayerMovement>().CDInRange.Remove(this);
        }
    }


    /*public void destroyOriginalBox()
    {
        gameObject.SetActive(false);
        createLinkedBox();
    }*/

    /*    public void createLinkedBox()
        {
            linkedBox.SetActive(true);
        }*/

    /*    public void destroyLinkedBox()
        {
            linkedBox.SetActive(false);
            createOriginalBox();
        }*/

    /*    public void createOriginalBox()
        {
            originalBox.SetActive(true);
        }*/
}
