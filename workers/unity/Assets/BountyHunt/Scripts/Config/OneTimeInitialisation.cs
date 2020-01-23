using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public static class OneTimeInitialisation
{
    private static bool initialized;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        PlayerLoopManager.RegisterDomainUnload(WorldsInitializationHelper.DomainUnloadShutdown, 1000);

        // Setup template to use for player on connecting client
        PlayerLifecycleConfig.CreatePlayerEntityTemplate = DonnerEntityTemplates.Player;
        PlayerLifecycleConfig.MaxNumFailedPlayerHeartbeats = 5;
    }
}