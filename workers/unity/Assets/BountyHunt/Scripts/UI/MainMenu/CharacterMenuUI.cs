using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;
using Daemon;
using QRCoder;
using QRCoder.Unity;
using System.Threading.Tasks;
using Bbhrpc;
using System.Threading;
using Lnrpc;
using Fps.Guns;
using Fps.Movement;

public class CharacterMenuUI : MonoBehaviour, IRefreshableUI
{
    public TextMeshProUGUI detailsHeaderText;
    public TextMeshProUGUI buyStateText;
    public Image detailsPreviewImage;
    public Button BuyAndEquipButton;
    public TextMeshProUGUI BuyAndEquipButtonText;
    public Button refreshButton;

    public GameObject ColorButtonsContainer;
    List<SkinColorButtonUI> skinColorButtons;
    public SkinColorButtonUI skinColorButtonPrefab;

    public SkinGroupButtonUI skinGroupButtonPrefab;
    public Transform skinGroupButtonsContainer;

    public TextMeshProUGUI BalanceText;

    public List<Toggle> WeaponButtonToggles;

    //SkinsLibrary playerSkinsLibrary;

    SkinItem selectedSkinItem;
    List<SkinGroupButtonUI> skinGroupButtons;
    SkinGroupButtonUI selectedSkinGroupButton;


    private bool isInit;
    private void Start()
    {
        refreshButton.onClick.AddListener(Refresh);
        //ClientEvents.instance.onServicesSetup.AddListener(Init);
        GetComponent<SlideSubMenuUI>().onActivate.AddListener(OnActivate);
        GetComponent<SlideSubMenuUI>().onDeactivate.AddListener(OnDeactivate);

        for (int i = 0;i<WeaponButtonToggles.Count;i++)
        {
            int id = i;
            WeaponButtonToggles[i].onValueChanged.AddListener((bool value) =>
            {
                if (value) SetGun(id);
            });
        }
        int gunId = PlayerPrefs.GetInt("SelectedGunID", PlayerGunSettings.DefaultGunIndex);
        WeaponButtonToggles[gunId].SetIsOnWithoutNotify(true);
        WeaponButtonToggles[gunId].onValueChanged.Invoke(true);
    }
    async Task Init()
    {
        ShopSkin[] shopSkins;
        SkinInventory inventory;
        long balance;
        try
        {
            shopSkins = await PlayerServiceConnections.instance.BackendPlayerClient.GetAllSkins();
            inventory = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInventory();
            var b = (await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance());
            balance = b.DaemonBalance;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }
        BalanceText.text = Utility.SatsToShortString(balance, true);

        await SkinShop.Refresh();

        skinColorButtons = ColorButtonsContainer.GetComponentsInChildren<SkinColorButtonUI>(true).ToList();
        
        skinGroupButtons = skinGroupButtonsContainer.GetComponentsInChildren<SkinGroupButtonUI>(true).ToList();

        selectedSkinItem = SkinShop.EquippedSkin;

        UpdateSkinGroupButtons();
        UpdateDetailsPanel();
    }
    public async void Refresh()
    {
        if (!isInit)
        {
            await Init();
            isInit = true;
        }

        StartCoroutine( refreshGunToggles());

        await RefreshTask();
    }
    async Task RefreshTask()
    {
        await SkinShop.Refresh();

        BalanceText.text = Utility.SatsToShortString(SkinShop.PlayerBalance,true);

        //selectedSkinItem = playerSkinsLibrary.GetSkin(selectedSkinItem.ID);

        UpdateSkinGroupButtons();
        UpdateDetailsPanel();
    }

    private void OnActivate()
    {
        if (PlayerServiceConnections.instance.ServicesReady)
        {
            Refresh();
        }
    }

    private void OnDeactivate()
    {
        if (PlayerServiceConnections.instance.ServicesReady)
        {
            PreviewSpot.Instance.SetSkin(SkinShop.EquippedSkin.skin);
        }
    }

    void UpdateSkinGroupButtons()
    {
        var groups = SkinShop.ItemGroupDict;
        SkinGroup equippedGroup = SkinShop.EquippedSkin.skin.group;
        SkinGroup selectedGroup = selectedSkinItem.skin.group;
        
        int counter = 0;
        foreach(var group in groups)
        {
            SkinGroupButtonUI sgb;
            if (counter < skinGroupButtons.Count)
            {
                sgb = skinGroupButtons[counter];
            }
            else
            {
                sgb = Instantiate(skinGroupButtonPrefab, skinGroupButtonsContainer);
                skinGroupButtons.Add(sgb);
            }
            sgb.gameObject.SetActive(true);
            sgb.set(group.Key, SelectSkinGroup);

            if (group.Key == equippedGroup) sgb.SetEquippedState(true);
            else sgb.SetEquippedState(false);

            if (group.Key == selectedGroup) sgb.SetSelection(true);
            else sgb.SetSelection(false);

            counter++;
        }

        for (int i = counter; i < skinGroupButtons.Count; i++)
        {
            skinGroupButtons[i].gameObject.SetActive(false);
        }
    }

    public void UpdateDetailsPanel()
    {
        SkinItem item = selectedSkinItem;
        detailsHeaderText.text = item.skin.group.groupName;
        detailsPreviewImage.sprite = item.skin.group.sprite;
        BuyAndEquipButton.interactable = true;

        BuyAndEquipButton.gameObject.SetActive(true);

        if (item.owned)
        {
            
            if (item == SkinShop.EquippedSkin)
            {
                buyStateText.text = "equipped";
                BuyAndEquipButton.gameObject.SetActive(false);
            }
            else
            {
                buyStateText.text = "owned";
                BuyAndEquipButtonText.text = "equip";
                BuyAndEquipButton.onClick.RemoveAllListeners();
                BuyAndEquipButton.onClick.AddListener(Equip);
            }
        }
        else
        {
            buyStateText.text = Utility.SatsToShortString(item.price, UITinter.tintDict[TintColor.Sats]);
            BuyAndEquipButtonText.text = "buy";
            BuyAndEquipButton.onClick.RemoveAllListeners();
            BuyAndEquipButton.onClick.AddListener(Buy);
        }
        UpdateSkinGroupColors(SkinShop.ItemGroupDict[item.skin.group]);
        PreviewSpot.Instance.SetSkin(item.skin);
    }

    public void UpdateSkinGroupColors(List<SkinItem> items)
    {
        
        for (int i = 0; i<items.Count || i < skinColorButtons.Count;i++)
        {
            if (i >= items.Count)
            {
                skinColorButtons[i].gameObject.SetActive(false);
                continue;
            }
            SkinItem item = items[i];


            SkinColorButtonUI scb;
            if (i >= skinColorButtons.Count)
            {
                scb = Instantiate(skinColorButtonPrefab, ColorButtonsContainer.transform);
                skinColorButtons.Add(scb);
            }
            else
            {
                scb = skinColorButtons[i];
            }
            skinColorButtons[i].gameObject.SetActive(true);
            scb.image.color = item.skin.identificationColor;
            scb.item = item;
            scb.lockedImage.gameObject.SetActive(!item.owned);

            if (SkinShop.EquippedSkin == item) scb.frame.SetActive(true);
            else scb.frame.SetActive(false);

            if (selectedSkinItem == item) scb.underLine.SetActive(true);
            else scb.underLine.SetActive(false);

            scb.onClick.RemoveAllListeners();
            scb.onClick.AddListener(selectSkin);
        }
    }


    private async void Buy()
    {
        SkinItem item = selectedSkinItem;
        SkinGroup grp = selectedSkinItem.skin.group;
        string res;
        try
        {
            res = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInvoice(item.skin.ID);

        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error",e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        GetBalanceResponse balanceResponse;

        try
        {
            balanceResponse = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        PayReq payreq;
        try
        {
            payreq = await PlayerServiceConnections.instance.lnd.DecodePayreq(res);
            
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }



        long balance = balanceResponse.DaemonBalance;

        string text1 = String.Format(GameText.BuySkinPopup, grp.groupName, Utility.SatsToShortString(item.price, UITinter.tintDict[TintColor.Sats]));
        Sprite sprite = grp.sprite;
        string text2 = "";
        if (balance < payreq.NumSatoshis) text2 = GameText.BuySkinPopupWarning;
        List<PopUpButtonArgs> actions = new List<PopUpButtonArgs>();
        actions.Add(new PopUpButtonArgs("ingame Wallet", () => BuyWithIngameWallet(res,payreq)));
        actions.Add(new PopUpButtonArgs("external Wallet", () => BuyWithExternalWallet(res, payreq)));

        ImagePopUpArgs args = new ImagePopUpArgs("buy skin", text1, sprite, text2, actions, false, false, 0.5f);
        PopUpUI popup = PopUpManagerUI.instance.OpenImagePopUp(args);

        popup.image.color = item.skin.identificationColor;
        if (balance < payreq.NumSatoshis) popup.buttons[0].interactable = false;

    }

    private async void BuyWithIngameWallet(string invoice, PayReq payreq)
    {
        PaymentUIHelper.IngamePayment(invoice, payreq, onSuccess: Refresh);
    }
    private async void BuyWithExternalWallet(string invoice, PayReq payreq)
    {
        PaymentUIHelper.ExternalPayment(invoice, payreq, onSuccess: Refresh);
    }

    private async void Equip()
    {
        // Todo check for errors
        try
        {
            await PlayerServiceConnections.instance.BackendPlayerClient.EquipSkin(selectedSkinItem.skin.ID);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);

        }

        Refresh();
    }

    public void SelectSkinGroup(SkinGroup group)
    {
        selectedSkinItem = SkinShop.ItemGroupDict[group][0];

        if (selectedSkinGroupButton != null) selectedSkinGroupButton.SetSelection(false);
        selectedSkinGroupButton = skinGroupButtons.Find(o => o.group == group);
        selectedSkinGroupButton.SetSelection(true);

        UpdateDetailsPanel();
        UpdateSkinGroupButtons();
    }

    public void selectSkin(SkinItem skinItem)
    {
        selectedSkinItem = skinItem;
        UpdateDetailsPanel();
    }

    public void SetGun(int id)
    {
        PreviewSpot.Instance.SetWeapon(id);
        PlayerPrefs.SetInt("SelectedGunID", id);
        PlayerPrefs.Save();
    }

    IEnumerator refreshGunToggles()
    {
        int gunId = PlayerPrefs.GetInt("SelectedGunID", PlayerGunSettings.DefaultGunIndex);
        yield return new WaitForEndOfFrame();
        WeaponButtonToggles[gunId].isOn = true;
    }

    private void OnEnable()
    {
        if(PlayerServiceConnections.instance.ServicesReady)Refresh();
    }

}

