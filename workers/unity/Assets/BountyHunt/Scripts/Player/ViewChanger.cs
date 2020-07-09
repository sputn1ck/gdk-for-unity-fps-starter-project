using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChanger : MonoBehaviour
{
    public GameObject FirstPersonModel;
    public GameObject ThirdPersonModel;

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
        this.thirdPerson = thirdPerson;

        ThirdPersonModel.SetActive(this.thirdPerson);
        FirstPersonModel.SetActive(!this.thirdPerson);
    }

}
