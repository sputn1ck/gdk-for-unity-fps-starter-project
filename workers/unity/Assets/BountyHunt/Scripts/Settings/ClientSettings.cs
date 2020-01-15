using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSettings : MonoBehaviour
{
    public static ClientSettings instance;


    public BountyPickUpMaterialSettings bountyPickupMaterialSettings;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
