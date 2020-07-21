using Fps;
using Fps.Guns;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChanger : MonoBehaviour, IRequiresGun
{
    public GameObject FirstPersonModel;
    public GameObject ThirdPersonModel;
    public Transform thirdPersonCameraSocket;

    // third person gun stuff
    [SerializeField] private Transform thirdPersonGunSocket;
    private GameObject thirdPersonGunModel;

    private Transform thirdPersonCameraTransform;
    private float currentZoom = 0.7f;

    const float startPitch = 30;
    const float maxPitch = 90;
    const float minPitch = 0;
    const float minZoom = -5f;
    const float maxZoom = -0.6f;
    const float zoomSpeed= 0.05f;
    public AnimationCurve zoomShiftYCurve;

    public bool thirdPerson { get; private set; }

    private void Awake()
    {
        thirdPersonCameraTransform = thirdPersonCameraSocket.GetComponentInChildren<Camera>().transform;
    }
    private void Start()
    {
        SetThirdPersonView(false);
    }

    [ContextMenu("switch view")]
    public void SwitchView()
    {
        SetThirdPersonView(!thirdPerson);
    }

    public void SetThirdPersonView(bool thirdPerson)
    {
        if (thirdPerson == this.thirdPerson) return;
        this.thirdPerson = thirdPerson;

        if (thirdPerson) {

            ResetThirdPersonView();
            BBHUIManager.instance.inGame.Reticle.showReticle(false);
        }
        else
        {
            BBHUIManager.instance.inGame.Reticle.showReticle(true);
        }

        ThirdPersonModel.SetActive(thirdPerson);
        FirstPersonModel.SetActive(!thirdPerson);

        //currentZoom = 0.3f;
    }

    void ResetThirdPersonView()
    {
        thirdPersonCameraSocket.localEulerAngles = new Vector3(startPitch, 0, 0);
    }

    public void UpdateThirdPersonView(float yawDelta,float pitchDelta, float zoomDelta)
    {
        Vector3 eul = thirdPersonCameraSocket.localEulerAngles;
        float y = eul.y + yawDelta;
        float x = Mathf.Clamp(eul.x - pitchDelta,minPitch,maxPitch);
        eul = new Vector3(x, y, 0);
        thirdPersonCameraSocket.localEulerAngles = eul;

        float zoomChange = zoomDelta * zoomSpeed;
        currentZoom = Mathf.Clamp01(currentZoom + zoomChange);
        float z = Mathf.Lerp(minZoom, maxZoom, currentZoom);
        thirdPersonCameraTransform.localPosition = new Vector3(0, 0, z);
        float yShift = zoomShiftYCurve.Evaluate(currentZoom);
        thirdPersonCameraTransform.position = thirdPersonCameraTransform.position + new Vector3(0, yShift, 0);
    }

    public void InformOfGun(GunSettings settings)
    {
        if (thirdPersonGunModel != null)
        {
            Destroy(thirdPersonGunModel);
        }
        thirdPersonGunModel = Instantiate(settings.GunModel, thirdPersonGunSocket);
        thirdPersonGunModel.transform.localPosition = Vector3.zero;
        thirdPersonGunModel.transform.localRotation = Quaternion.identity;
    }
}
