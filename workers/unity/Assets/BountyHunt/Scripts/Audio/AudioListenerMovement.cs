using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerMovement : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        ClientEvents.instance.onPlayerDie.AddListener(moveToManager);
        ClientEvents.instance.onPlayerSpawn.AddListener(moveToPlayer);
    }

    void moveToPlayer(GameObject obj)
    {

        transform.SetParent(obj.GetComponent<Fps.Movement.FpsDriver>().camera.transform);
        transform.resetLocalTransform();

    }

    void moveToManager()
    {
        transform.SetParent(AudioManager.instance.transform);
        transform.resetLocalTransform();
    }

}
