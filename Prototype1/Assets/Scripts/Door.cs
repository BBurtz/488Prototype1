using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * Author: Sky Beal
 * Description: Sets corresponding door to "off".
 */
public class Door : MonoBehaviour
{
    [Tooltip ("Exact string for the level the door takes you to.")]
    public string NextScene;

    [Tooltip("Material that is applied to the door after getting a key.")]
    public Material HalfOpacityMaterial;

    //Door Mesh Renderer
    private MeshRenderer mr;


    private void Start()
    {
       mr = gameObject.GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Changes door's material
    /// </summary>
    public void OpenDoor()
    {
        mr.material = HalfOpacityMaterial;
    }

    /// <summary>
    /// Loads next scene
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}
