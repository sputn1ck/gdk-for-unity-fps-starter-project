using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundSpawner : MonoBehaviour
{
    public Transform leftFoot;
    public Transform rightFoot;
    public float footStepVolume= 0.5f;
    public List<AudioClip> stepSounds;

    public void TriggerFootStepSound(int footID)
    {
        int soundID = Random.Range(0, stepSounds.Count);
        Vector3 footPosition = footID == 0 ? leftFoot.position : rightFoot.position;
        AudioManager.instance.spawnSound(stepSounds[soundID], footPosition,volume:footStepVolume);
    }
}
