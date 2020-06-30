using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtLink : MonoBehaviour, ILookAtHandler
{
    
    public GameObject target;

    public void OnLookAtEnter()
    {
        ILookAtHandler[] lookatHandlers = target.GetComponents<ILookAtHandler>();
        foreach(ILookAtHandler lah in lookatHandlers)
        {
            lah.OnLookAtEnter();
        }
    }

    public void OnLookAtExit()
    {
        ILookAtHandler[] lookatHandlers = target.GetComponents<ILookAtHandler>();
        foreach (ILookAtHandler lah in lookatHandlers)
        {
            lah.OnLookAtExit();
        }
    }
}
