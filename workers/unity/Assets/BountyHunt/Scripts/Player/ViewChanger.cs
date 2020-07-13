using Fps;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChanger : MonoBehaviour
{

    public GameObject FirstPersonModel;
    public GameObject ThirdPersonModel;

    public Transform thirdPersonCameraSocket;

    const float startPitch = 30;
    const float maxPitch = 90;
    const float minPitch = 0;

    public bool thirdPerson { get; private set; }

    private void Awake()
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
    }

    void ResetThirdPersonView()
    {
        thirdPersonCameraSocket.localEulerAngles = new Vector3(startPitch, 0, 0);
    }

    public void UpdateThirdPersonView(float yawDelta,float pitchDelta)
    {
        Vector3 eul = thirdPersonCameraSocket.localEulerAngles;
        float y = eul.y + yawDelta;
        float x = Mathf.Clamp(eul.x - pitchDelta,minPitch,maxPitch);
        eul = new Vector3(x, y, 0);
        thirdPersonCameraSocket.localEulerAngles = eul;
    }
}
