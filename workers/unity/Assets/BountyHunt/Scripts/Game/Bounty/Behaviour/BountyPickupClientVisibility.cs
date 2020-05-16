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
        public AudioClip collectSound;
        public bool showAdvertiser;
        public GameObject holoAdParent;
        private MeshRenderer cubeMeshRenderer;

        private void OnEnable()
        {
            cubeMeshRenderer = GetComponentInChildren<MeshRenderer>();
            bountyPickUpReader.OnUpdate += OnBountyPickupComponentUpdated;
            UpdateVisibility();
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
            Debug.Log("bounty update. Active: " +  update.IsActive);

            UpdateVisibility();
            if(update.IsActive == false)
            {
                AudioManager.instance.spawnSound(collectSound, transform.position);
            }
        }

        private void OnDisable()
        {
            bountyPickUpReader.OnUpdate -= OnBountyPickupComponentUpdated;
        }

    }
}
