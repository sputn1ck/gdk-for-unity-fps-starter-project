using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fps;
using Fps.Guns;
using Fps.Movement;

public class InGameScreenManagerUI : MonoBehaviour
{
    public ScreenUI RespawnScreen;
    public ScreenUI Hud;
    public ScreenUI EscapeScreen;
    public ScreenUI ScoreBoardScreen;
    public GameObject Reticle;

    //TODO
    //public GameObject BountyUi;
    //public GameObject EarningsUi;
    //public GameObject GameStats;
    //public GameObject ChatPanelUI;
    //public GameObject LnInfoPanel;
    //public GameObject DonationWindow;

    //TODO
    //public Animator HitMarkerAnimator;

    //TODO
    //public bool isChatting;

    private bool isPlayerAiming;


    private void Start()
    {
        ClientShooting.instance.OnPlayerHit.AddListener(ShowHitmarker);
    }

    public void OnEnable()
    {
        
        RespawnScreen.gameObject.SetActive(true);
        EscapeScreen.gameObject.SetActive(true);
        ScoreBoardScreen.gameObject.SetActive(true);
        Hud.gameObject.SetActive(true);

        RespawnScreen.activated = false;
        Hud.activated = true;
        EscapeScreen.activated = false;
        ScoreBoardScreen.activated = false;

        UpdateScreens();

        Reticle.SetActive(true);
    }


    public void OnDisable()
    {
        isPlayerAiming = false;
    }

    public void ShowHitmarker()
    {
        //TODO
        //HitMarkerAnimator.SetTrigger("show");
    }

    public void ToggleEscapeScreen()
    {
        EscapeScreen.activated = !EscapeScreen.activated;
        UpdateScreens();

    }

    public void SetEscapeScreen(bool active)
    {
        EscapeScreen.activated = active;
        UpdateScreens();

    }
    public void SetRespawnScreen(bool active)
    {
        RespawnScreen.activated = active;
        UpdateScreens();
    }

    public void SetPlayerAiming(bool isAiming)
    {
        isPlayerAiming = isAiming;
        Reticle.SetActive(!isPlayerAiming);
    }

    public void SetScoreBoardScreen(bool active)
    {
        ScoreBoardScreen.activated = active;
    }


    void UpdateScreens()
    {
        if (ScoreBoardScreen.activated)
        {
            ScoreBoardScreen.Show(true);
            EscapeScreen.Show(false);
            RespawnScreen.Show(false);
            Reticle.SetActive(false);
            Cursor.visible = true;
            StartCoroutine(SetCursorLockstateAtFrameEnd(CursorLockMode.None));
        }
        else if (EscapeScreen.activated)
        {
            ScoreBoardScreen.Show(false);
            EscapeScreen.Show(true);
            RespawnScreen.Show(false);
            Reticle.SetActive(!isPlayerAiming);
            Cursor.visible = true;
            StartCoroutine(SetCursorLockstateAtFrameEnd(CursorLockMode.None));
        }
        else if (RespawnScreen.activated)
        {
            ScoreBoardScreen.Show(false);
            EscapeScreen.Show(false);
            RespawnScreen.Show(true);
            Reticle.SetActive(!isPlayerAiming);
            Cursor.visible = true;
            StartCoroutine(SetCursorLockstateAtFrameEnd(CursorLockMode.None));
        }
        else
        {
            ScoreBoardScreen.Show(false);
            EscapeScreen.Show(false);
            RespawnScreen.Show(false);
            Cursor.visible = false;
            StartCoroutine(SetCursorLockstateAtFrameEnd(CursorLockMode.Locked));
        }

        Hud.Show(true);
    }


    IEnumerator SetCursorLockstateAtFrameEnd(CursorLockMode mode)
    {
        yield return new WaitForEndOfFrame();
        Cursor.lockState = mode;
    }

    private void Update()
    {
        Debug.Log("esc:" + EscapeScreen.activated + " rspwn:" + RespawnScreen.activated + " sb:" + ScoreBoardScreen.activated + " hud" + Hud.activated);

        if (EscapeScreen.activated){
            ClientMovementDriver.instance.ApplyMovement(Vector3.zero, transform.rotation, MovementSpeed.Run, false);
            FpsDriver.instance.Animations(false);
        }
    }

}
