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

    public void destroyBox()
    {
        gameObject.SetActive(false);
        createBox();
    }

    public void createBox()
    {
        if (gameObject == null)
        {
            linkedBox.SetActive(true);
        }
    }
}
