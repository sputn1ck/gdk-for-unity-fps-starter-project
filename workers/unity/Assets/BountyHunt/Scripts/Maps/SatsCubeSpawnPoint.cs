using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatsCubeSpawnPoint : MonoBehaviour
{
    public float weighting = 1;
    public bool bountybossSpawn;
    public string bountybossName;
    private void OnDrawGizmos()
    {
        Color c = Color.Lerp(Color.red, Color.green, weighting / 2);
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.color = c*new Color(1,1,1,0.4f);
        Gizmos.DrawCube(new Vector3(0, 1, 0), new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.color = c;
        Gizmos.DrawWireCube(new Vector3(0, 1, 0), new Vector3(0.5f, 0.5f, 0.5f));

    }
}
