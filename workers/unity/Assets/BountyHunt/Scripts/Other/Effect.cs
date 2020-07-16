using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public string key;

    public bool selfDestroy = true;

    public float LifetTime = 10;

    private void Start()
    {
        if (selfDestroy) StartCoroutine(DestroyEnumerator());
    }

    IEnumerator DestroyEnumerator()
    {
        yield return new WaitForSeconds(LifetTime);
        Destroy(this);
    }

}
