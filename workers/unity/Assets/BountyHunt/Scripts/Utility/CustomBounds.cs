using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CustomBounds : MonoBehaviour
{
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] Vector3 extent = new Vector3(3,3,3);
    [HideInInspector] public Bounds bounds => new Bounds(transform.position + offset, extent );
}


[CustomEditor(typeof(CustomBounds))]
public class CustomBoundsEditor : Editor
{
    public bool HasFrameBounds()
    {
        return true;
    }

    public Bounds OnGetFrameBounds()
    {
        CustomBounds customBounds = (CustomBounds)target;
        Bounds bounds = customBounds.bounds;
        return bounds;
    }
}

