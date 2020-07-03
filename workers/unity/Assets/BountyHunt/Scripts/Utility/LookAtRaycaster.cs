using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LookAtRaycaster : MonoBehaviour
{
    public float minDistance = 0.3f;
    public float maxDistance = 100f;

    Transform target;
    private void Update()
    {
        if (!Camera.main) return;

        Transform newTarget;
        Ray ray = new Ray(Camera.main.transform.position + Camera.main.transform.forward * minDistance, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,maxDistance))
        {
            newTarget = hit.transform;
        }
        else newTarget = null;

        if(newTarget != target)
        {
            if(target != null)
            {
                ILookAtHandler[] exithandlers = target.GetComponentsInChildren<ILookAtHandler>();
                foreach (var eh in exithandlers)
                {
                    eh.OnLookAtExit();
                }
            }

            if (newTarget != null)
            {
                ILookAtHandler[] enterhandlers = newTarget.GetComponentsInChildren<ILookAtHandler>();
                foreach (var eh in enterhandlers)
                {
                    eh.OnLookAtEnter();
                }
            }
            target = newTarget;
        }
    }

    private void OnDisable()
    {
        
    }
}

public interface ILookAtHandler
{
    void OnLookAtEnter();
    void OnLookAtExit();

}

