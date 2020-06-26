using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CenterRaycaster : MonoBehaviour
{
    Transform target;
    public Camera cam;

    private void Update()
    {
        Transform newTarget;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            newTarget = hit.transform;
            
        }
        else newTarget = null;

        if(newTarget != target)
        {
            if(target != null)
            {
                IPointerExitHandler[] exithandlers = target.GetComponentsInChildren<IPointerExitHandler>();
                foreach (var eh in exithandlers)
                {
                    eh.OnPointerExit(new PointerEventData(EventSystem.current));
                }
            }

            if (newTarget != null)
            {
                IPointerEnterHandler[] enterhandlers = newTarget.GetComponentsInChildren<IPointerEnterHandler>();
                foreach (var eh in enterhandlers)
                {
                    eh.OnPointerEnter(new PointerEventData(EventSystem.current));
                }
            }
            target = newTarget;
        }
    }

    private void OnDisable()
    {
        
    }
}
