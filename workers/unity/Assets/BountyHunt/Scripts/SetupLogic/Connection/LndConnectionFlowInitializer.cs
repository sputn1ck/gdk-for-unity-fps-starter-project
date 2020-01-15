using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LndConnectionFlowInitializer : IConnectionFlowInitializer<LocatorFlow>
{
    private readonly IConnectionFlowInitializer<LocatorFlow> initializer;

    public LndConnectionFlowInitializer(IConnectionFlowInitializer<LocatorFlow> standaloneInitializer)
    {
        initializer = standaloneInitializer;
    }

    public void Initialize(LocatorFlow flow)
    {
        initializer.Initialize(flow);
    }
}
