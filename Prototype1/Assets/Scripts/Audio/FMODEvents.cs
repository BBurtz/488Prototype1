using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Test")]
    [field: SerializeField] public EventReference TestSFX { get; private set; }

    [field: Header("PlayerSFX")]
    [field: SerializeField] public EventReference Jump { get; private set; }
    [field: SerializeField] public EventReference Shift { get; private set; }
    [field: SerializeField] public EventReference Walk { get; private set; }

    [field: Header("EnvironmentSFX")]
    [field: SerializeField] public EventReference BoxBlocked { get; private set; }
    [field: SerializeField] public EventReference BoxDestroy { get; private set; }
    [field: SerializeField] public EventReference BoxMove { get; private set; }
    [field: SerializeField] public EventReference Key { get; private set; }
    [field: SerializeField] public EventReference PlateDown { get; private set; }
    [field: SerializeField] public EventReference PlateUp { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("There is more than one FMODEvents in the scene");
        }
        instance = this;
    }
}
