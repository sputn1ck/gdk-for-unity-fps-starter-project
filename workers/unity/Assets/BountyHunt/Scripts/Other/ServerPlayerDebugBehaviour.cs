using Fps;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Core;

public class ServerPlayerDebugBehaviour : MonoBehaviour
{

    [Require] private HealthComponentCommandSender healthCommandSender;
    [Require] private HunterComponentWriter hunter;
    [Require] private EntityId entityId;
    [Require] private GunComponentWriter GunComponentWriter;

    public bool modifyHealthTrigger;
    public float modifyHealthAmount;

    public bool modifyBountyTrigger;
    public long modifiyBounty;

    public bool changeGunTrigger;
    public int gunId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (modifyHealthTrigger)
        {
            modifyHealthTrigger = false;
            ModifyHealth();
        }
        if (modifyBountyTrigger)
        {
            modifyBountyTrigger = false;
            AddBounty();
        }
        if(changeGunTrigger)
        {
            changeGunTrigger = false;
            ChangeGun();
        }
    }

    public void ChangeGun()
    {
        GunComponentWriter.SendUpdate(new GunComponent.Update()
        {
            GunId = this.gunId
        });
    }
    public void ModifyHealth()
    {
        healthCommandSender.SendModifyHealthCommand(entityId, new HealthModifier(0, modifyHealthAmount, new Fps.Vector3Int(0, 0, 0), new Fps.Vector3Int(0, 0, 0), entityId));

    }

    public void AddBounty()
    {

        //hunter.SendUpdate(new HunterComponent.Update { Bounty = hunter.Data.Bounty + modifiyBounty });
    }
}
