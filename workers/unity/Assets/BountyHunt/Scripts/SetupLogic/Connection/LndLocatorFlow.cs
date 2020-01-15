using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LndLocatorFlow : LocatorFlow
{
    private readonly string targetDeployment;

    public LndLocatorFlow(string targetDeployment, string loginToken, string playerIdentityToken,
        IConnectionFlowInitializer<LocatorFlow> initializer = null) : base(initializer)
    {
        this.targetDeployment = targetDeployment;
        this.UseDevAuthFlow = false;
        this.LoginToken = loginToken;
        this.PlayerIdentityToken = playerIdentityToken;
    }

    
}
