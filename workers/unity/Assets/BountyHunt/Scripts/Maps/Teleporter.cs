using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [ContextMenuItem("Reset","ResetDestinationTransforms")]
    public Vector3 destinationPosition;
    [Range(0,360)]
    public float destinationAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.4f, 0.7f, 1f);
        Gizmos.DrawLine(transform.position, destinationPosition);
        Quaternion qua = Quaternion.Euler(0, destinationAngle, 0);
        Gizmos.matrix = Matrix4x4.TRS(destinationPosition, qua, new Vector3(1, 0.02f, 1));
        Gizmos.DrawWireSphere(new Vector3(0, 0, 0), 0.5f);
        Gizmos.DrawLine(new Vector3(0, 0, 0.8f), new Vector3(0.1f, 0, 0.7f));
        Gizmos.DrawLine(new Vector3(0, 0, 0.8f), new Vector3(-0.1f, 0, 0.7f));
        Gizmos.color = new Color(0.4f, 0.7f, 1f,0.1f);
        Gizmos.DrawSphere(new Vector3(0, 0, 0), 0.5f);

    }
    public void ResetDestinationTransforms()
    {
        destinationPosition = transform.position+ Vector3.forward*2;
        destinationAngle = 0;
    }
}

