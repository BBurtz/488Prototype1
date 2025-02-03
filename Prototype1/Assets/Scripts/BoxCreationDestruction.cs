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

    public void destroyBox()
    {
        if (originalBox == true)
        {
            originalBox.SetActive(false);
            createLinkedBox();
        }
        else if (linkedBox == true)
        {
            linkedBox.SetActive(false);
            createOriginalBox();
        }
        else
        {
            //do nothing
        }
    }

    /*public void destroyOriginalBox()
    {
        gameObject.SetActive(false);
        createLinkedBox();
    }*/

    public void createLinkedBox()
    {
        linkedBox.SetActive(true);
    }

/*    public void destroyLinkedBox()
    {
        linkedBox.SetActive(false);
        createOriginalBox();
    }*/

    public void createOriginalBox()
    {
        originalBox.SetActive(true);
    }
}
