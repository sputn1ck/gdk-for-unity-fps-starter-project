using Improbable.Gdk.Subscriptions;
using UnityEngine;
using System.Collections.Generic;
using Fps.Config;
using Bountyhunt;

namespace Fps
{
    [WorkerType(WorkerUtils.UnityClient)]
    public class BountyPickupClientVisibility : MonoBehaviour
    {

        [Require] private BountyPickupReader bountyPickUpReader;

        public BountyPickUpMaterialSettings materialSettings;
        public AudioClip spawnSound;
        public bool showAdvertiser;
        public GameObject holoAdParent;
        private MeshRenderer cubeMeshRenderer;

        private void OnEnable()
        {
            cubeMeshRenderer = GetComponentInChildren<MeshRenderer>();
            bountyPickUpReader.OnUpdate += OnBountyPickupComponentUpdated;
            UpdateVisibility();
            //bountyPickUpReader.Data.BountyValue;
        }

        private void UpdateVisibility()
        {
            cubeMeshRenderer.enabled = bountyPickUpReader.Data.IsActive;
            BountyAppearence appearance = materialSettings.getMaterialByValue(bountyPickUpReader.Data.BountyValue); ;
            cubeMeshRenderer.material = appearance.mat;


            //transform.localScale = Vector3.one * appearance.scale;

        }
        

        private void OnBountyPickupComponentUpdated(BountyPickup.Update update)
        {
            UpdateVisibility();
            if(update.IsActive == false)
            {
                // TODO READD AUDIOMANAGER
                //AudioManager.instance.spawnSound(spawnSound, transform.position);
            }
        }

        /*
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                AudioObjectSpawner.instance.spawnSound(spawnSound, transform.position);
            }
        }*/
    }
}
