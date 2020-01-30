using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyAndEarningsUI : MonoBehaviour
{
    public BountyUIController BountyUI;
    public BountyUIController EarningsUI;


    // Start is called before the first frame update
    void Start()
    {
        ClientEvents.instance.onBountyUpdate.AddListener(UpdateBounty);
        ClientEvents.instance.onEarningsUpdate.AddListener(UpdateEarnings);

    }

    private void UpdateBounty(BountyUpdateEventArgs args)
    {
        BountyUI.UpdateSats(args.NewAmount, args.NewAmount - args.OldAmount);
    }

    private void UpdateEarnings(EarningsUpdateEventArgs args)
    {
        EarningsUI.UpdateSats(args.NewAmount, args.NewAmount - args.OldAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
