using System.Collections.Generic;
using UnityEngine;
/*
 * Author: Sky Beal
 * Description: Pressure Plate behaviour. (De)Activates objects and/or switches treadmill direction.
 */
public class PressurePlate : MonoBehaviour
{
    [Header ("What is Affected")]

    [Tooltip ("Will turn objects in the list to opposite activation status.")]
    public bool TurnsOnOrOff = true;

    [Tooltip("Will switch treadmills in the list to opposite direction.")]
    public bool SwitchesTreadmills = true;

    [Header("List of Objects Affected")]

    [Tooltip("List of objects that will be set to opposite activation status.")]
    public List<GameObject> AffectedObjectsForOnAndOff = new List<GameObject>();

    [Tooltip("List of treadmills that will be set to opposite direction.")]
    public List<GameObject> AffectedTreadmills = new List<GameObject>();
    
    /// <summary>
    /// Detects box on pressure plate.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //registering only the trigger collider
        if (other.isTrigger)
        {
            //registering only boxes
            if (other.GetComponent<BoxBehavior>())
            {
                //only calls when turning on or off
                if (TurnsOnOrOff)
                {
                    Activator();
                }

                //only calls when switching treadmills
                if (SwitchesTreadmills)
                { 
                    TreadmillSwitch();
                }
                    
            }
        }
    }

    /// <summary>
    /// Detects box leaving pressure plate.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        //registering only the trigger collider
        if (other.isTrigger)
        {
            //registering only boxes
            if (other.GetComponent<BoxBehavior>())
            {
                //only calls when turning on or off
                if (TurnsOnOrOff)
                {
                    Activator();
                }

                //only calls when switching treadmills
                if (SwitchesTreadmills)
                {
                    TreadmillSwitch();
                }

            }
        }
    }

    /// <summary>
    /// Changes objects' activation status to the opposite.
    /// Off will turn on, on will turn off.
    /// </summary>
    private void Activator()
    {
        foreach (GameObject objectAffected in AffectedObjectsForOnAndOff)
        {
            //null check
            if(objectAffected == null)
            {
                return;
            }

            //turn off if on
            if (objectAffected.activeSelf)
            {
                objectAffected.SetActive(false);
            }

            //turn on if off
            else
            {
                objectAffected.SetActive(true);
            }    
        }
    }

    /// <summary>
    /// Changes treadmills' direction to the opposite.
    /// </summary>
    private void TreadmillSwitch()
    {
        foreach (GameObject treadmill in AffectedTreadmills)
        {
            //null check
            if (treadmill == null)
            {
                return;
            }

            //changes direction
            TreadmillBehavior TB = treadmill.GetComponent<TreadmillBehavior>();
            TB.FlipTreadmillDirection();
        }
    }
}
