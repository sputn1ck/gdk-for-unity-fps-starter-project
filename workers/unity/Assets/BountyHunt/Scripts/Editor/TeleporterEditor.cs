using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Teleporter))][ExecuteInEditMode]
public class TeleporterEditor :  Editor
{
    private void OnEnable()
    {
        
    }

    private void OnSceneGUI()
    {
        Debug.Log("Editor!!!!");
        Teleporter teleporter = target as Teleporter;
        Quaternion qua = Quaternion.Euler(0, teleporter.destinationAngle, 0);


        //Handles.color = new Color(0.3f, 0.6f, 1);
        teleporter.destinationPosition = Handles.PositionHandle(teleporter.destinationPosition, Quaternion.identity);
        teleporter.destinationAngle = Handles.Disc(qua,teleporter.destinationPosition, Vector3.up, 0.7f, false, 1).eulerAngles.y;
        
        Vector3 arrowPos = teleporter.destinationPosition + qua * Vector3.forward * 0.7f;

    }
}
