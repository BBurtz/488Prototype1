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
        }
        else if (!originalBox.activeInHierarchy && linkedBox.activeInHierarchy)
        {
            originalBox.SetActive(true);
            linkedBox.SetActive(false);
        }
        else
        {
            //do nothing
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
