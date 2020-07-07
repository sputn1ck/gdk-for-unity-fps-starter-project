using System;
using System.Collections;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
using UnityEngine;
using Fps;
using Fps.WorldTiles;

public abstract class DonnerWorkerConnectorBase : WorkerConnector
    {
        public int TargetFrameRate = 60;

        [SerializeField] protected MapTemplate mapTemplate;

        [NonSerialized] public GameObject LevelInstance;

        protected abstract IConnectionHandlerBuilder GetConnectionHandlerBuilder();

        protected virtual void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
        }

        protected async Task AttemptConnect()
        {
            await Connect(GetConnectionHandlerBuilder(), new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            //StartCoroutine(LoadWorld());
        }

        public override void Dispose()
        {
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
                LevelInstance = null;
            }

            base.Dispose();
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        protected virtual IEnumerator LoadWorld()
        {
            // Defer a frame to allow worker flags to propagate.
            yield return null;

            var worldSize = GetWorldSize();
            if (worldSize <= 0)
            {
                yield break;
            }

            yield return DonnerMapBuilder.GenerateMap(
                mapTemplate,
                worldSize,
                transform,
                Worker.WorkerType,
                this);
        ClientEvents.instance.onMapLoaded.Invoke();
        }

        protected int GetWorldSize()
        {
            var flagValue = Worker.GetWorkerFlag("world_size");

            if (!int.TryParse(flagValue, out var worldSize))
            {
                Worker.LogDispatcher.HandleLog(LogType.Error,
                    new LogEvent($"Invalid world_size worker flag. Expected an integer, got \"{flagValue}\""));
                return 0;
            }

            return worldSize;
        }
    }

