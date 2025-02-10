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
    [SerializeField] bool isActive;

    
    public void destroyBox()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        if (originalBox.activeInHierarchy && !linkedBox.activeInHierarchy)
        {
            linkedBox.SetActive(true);
            originalBox.SetActive(false);
            pm.CDInRange.Remove(originalBox.GetComponent<BoxCreationDestruction>());
            pm.BoxesInRange.Remove(originalBox.GetComponent<BoxBehavior>());

            Debug.Log("Does destroying original work?");
        }
        else if (!originalBox.activeInHierarchy && linkedBox.activeInHierarchy)
        {
            originalBox.SetActive(true);
            linkedBox.SetActive(false);
            pm.CDInRange.Remove(linkedBox.GetComponent<BoxCreationDestruction>());
            pm.BoxesInRange.Remove(linkedBox.GetComponent<BoxBehavior>());

        }
        else
        {
            Debug.LogWarning("Warning! Make sure one of the boxes is inactive in the hierarchy");
        }
    }
    

    /*public void destroyBox()
    {
        isActive = !isActive;
        if(!isActive)
        {
            originalBox.GetComponent<MeshRenderer>().material = deactiveM;
            linkedBox.GetComponent<MeshRenderer>().material = origM;
        }
        else
        {
            linkedBox.GetComponent<MeshRenderer>().material = deactiveM;
            originalBox.GetComponent<MeshRenderer>().material = origM;
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        //Check if the object has player movement
        if (other.GetComponent<PlayerMovement>() != null && !other.gameObject.GetComponent<PlayerMovement>().BoxesMoveFreely)
        {
                //Add the current box to the player's box list
                other.GetComponent<PlayerMovement>().CDInRange.Add(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null && !other.gameObject.GetComponent<PlayerMovement>().BoxesMoveFreely)
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
