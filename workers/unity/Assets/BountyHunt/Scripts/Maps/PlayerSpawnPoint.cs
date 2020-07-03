using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.color = new Color(0.4f, 0.6f, 1f,0.4f);
        Gizmos.DrawCube(new Vector3(0,0.85f, 0), new Vector3(0.4f, 1.7f, 0.4f));
        Gizmos.color = new Color(0.4f, 0.6f, 1f, 1);
        Gizmos.DrawWireCube(new Vector3(0,0.85f, 0), new Vector3(0.4f, 1.7f, 0.4f));

        Gizmos.DrawLine(new Vector3(0.2f, 0, 0.2f), Vector3.forward * 0.5f);
        Gizmos.DrawLine(new Vector3(-0.2f, 0, 0.2f), Vector3.forward * 0.5f);
        Gizmos.DrawLine(new Vector3(0.2f, 1.7f, 0.2f), new Vector3(0, 1.7f, 0) + Vector3.forward * 0.5f);
        Gizmos.DrawLine(new Vector3(-0.2f, 1.7f, 0.2f), new Vector3(0, 1.7f, 0) + Vector3.forward * 0.5f);


    }
}
