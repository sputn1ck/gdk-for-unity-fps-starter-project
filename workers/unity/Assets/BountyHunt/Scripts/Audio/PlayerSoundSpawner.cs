using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundSpawner : MonoBehaviour
{
    public Transform leftFoot;
    public Transform rightFoot;

    public List<AudioClip> stepSounds;

    public void TriggerFootStepSound(int footID)
    {
        int soundID = Random.Range(0, stepSounds.Count);
        Transform foot = footID == 0 ? leftFoot : rightFoot;
        AudioManager.instance.spawnSound(stepSounds[soundID], foot);
    }
}
