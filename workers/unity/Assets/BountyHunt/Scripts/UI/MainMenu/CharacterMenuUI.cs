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

public class CharacterMenuUI : MonoBehaviour
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

    SkinsLibrary playerSkinsLibrary;

    Skin selectedSkin;
    List<SkinGroupButtonUI> skinGroupButtons;
    SkinGroupButtonUI selectedSkinGroupButton;
    Skin equippedSkin;

    private void Start()
    {
        refreshButton.onClick.AddListener(Refresh);
        ClientEvents.instance.onServicesSetup.AddListener(Init);
    }
    async void Init()
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

        playerSkinsLibrary = Instantiate(SkinsLibrary.MasterInstance);
        playerSkinsLibrary.InitializeForCharacterMenu(shopSkins, inventory.OwnedSkins.ToArray());

        skinColorButtons = ColorButtonsContainer.GetComponentsInChildren<SkinColorButtonUI>(true).ToList();
        
        skinGroupButtons = skinGroupButtonsContainer.GetComponentsInChildren<SkinGroupButtonUI>(true).ToList();
        equippedSkin = playerSkinsLibrary.GetSkin(inventory.EquippedSkin);
        selectedSkin = playerSkinsLibrary.GetSkin(PlayerPrefs.GetString("EquippedSkinID", playerSkinsLibrary.defaultSkinID));

        UpdateSkinGroupButtons();
        UpdateDetailsPanel();
    }
    async void Refresh()
    {
        await RefreshTask();
    }
    async Task RefreshTask()
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

        BalanceText.text = Utility.SatsToShortString(balance,true);

        playerSkinsLibrary = Instantiate(SkinsLibrary.MasterInstance);
        playerSkinsLibrary.InitializeForCharacterMenu(shopSkins, inventory.OwnedSkins.ToArray());

        equippedSkin = playerSkinsLibrary.GetSkin(inventory.EquippedSkin);
        selectedSkin = playerSkinsLibrary.GetSkin(selectedSkin.ID);

        UpdateSkinGroupButtons();
        UpdateDetailsPanel();
    }

    void UpdateSkinGroupButtons()
    {
        List<SkinGroup> groups = playerSkinsLibrary.groups;
        SkinGroup equippedGroup = playerSkinsLibrary.GetGroup(PlayerPrefs.GetString("EquippedSkinID", playerSkinsLibrary.defaultSkinID));
        SkinGroup selectedGroup = playerSkinsLibrary.GetGroup(selectedSkin.ID);
        int counter = 0;
        foreach (SkinGroupButtonUI sgb in skinGroupButtons)
        {
            if (groups.Count > counter)
            {
                sgb.gameObject.SetActive(true);
                sgb.set(groups[counter], SelectSkinGroup);

                if (groups[counter] == equippedGroup) sgb.SetEquippedState(true);
                else sgb.SetEquippedState(false);

                if (groups[counter] == selectedGroup) sgb.SetSelection(true);
                else sgb.SetSelection(false);

            }
            else
            {
                sgb.gameObject.SetActive(false);
            }
            counter++;
        }
        for (int i = counter; i < groups.Count; i++)
        {
            SkinGroupButtonUI sgb = Instantiate(skinGroupButtonPrefab, skinGroupButtonsContainer);
            sgb.gameObject.SetActive(true);
            sgb.set(groups[i], SelectSkinGroup);

            if (groups[i] == equippedGroup) sgb.SetEquippedState(true);
            else sgb.SetEquippedState(false);

            if (groups[i] == selectedGroup) sgb.SetSelection(true);
            else sgb.SetSelection(false);
        }
    }

    public void UpdateDetailsPanel()
    {
        Skin skin = selectedSkin;
        detailsHeaderText.text = skin.group.groupName;
        detailsPreviewImage.sprite = skin.group.sprite;
        BuyAndEquipButton.interactable = true;

        BuyAndEquipButton.gameObject.SetActive(true);

        if (skin.owned)
        {
            
            if (skin == equippedSkin)
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
            buyStateText.text = Utility.SatsToShortString(skin.price, UITinter.tintDict[TintColor.Sats]);
            BuyAndEquipButtonText.text = "buy";
            BuyAndEquipButton.onClick.RemoveAllListeners();
            BuyAndEquipButton.onClick.AddListener(Buy);
        }
        UpdateSkinGroupColors(skin.group);
        PreviewSpot.Instance.SetSkin(skin);
    }

    public void UpdateSkinGroupColors(SkinGroup group)
    {
        
        for (int i = 0; i<group.skins.Count || i < skinColorButtons.Count;i++)
        {
            if (i >= group.skins.Count)
            {
                skinColorButtons[i].gameObject.SetActive(false);
                continue;
            }
            Skin skn = group.skins[i];


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
            scb.image.color = skn.identificationColor;
            scb.skin = skn;
            scb.lockedImage.gameObject.SetActive(!skn.owned);

            if (equippedSkin == skn) scb.frame.SetActive(true);
            else scb.frame.SetActive(false);

            if (selectedSkin == skn) scb.underLine.SetActive(true);
            else scb.underLine.SetActive(false);

            scb.onClick.RemoveAllListeners();
            scb.onClick.AddListener(selectSkin);
        }
    }


    private async void Buy()
    {
        Skin skn = selectedSkin;
        SkinGroup grp = selectedSkin.group;
        string res;
        try
        {
            res = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInvoice(skn.ID);

        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error",e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        GetBalanceResponse balanceResponse = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
        long balance = balanceResponse.DaemonBalance;

        string text1 = "You are going to buy: \n " + grp.groupName +" \n for "+ Utility.SatsToShortString(skn.price,UITinter.tintDict[TintColor.Sats]);
        Sprite sprite = grp.sprite;
        string text2 = "";
        if (balance < skn.price) text2 = "Your Ingame Wallet doesent cover the required amount!"; //Todo hint when there is no channel, or to less balance
        List<PopUpButtonArgs> actions = new List<PopUpButtonArgs>();
        actions.Add(new PopUpButtonArgs("ingame Wallet", () => BuyWithIngameWallet(res)));
        actions.Add(new PopUpButtonArgs("external Wallet", () => BuyWithExternalWallet(res)));

        ImagePopUpArgs args = new ImagePopUpArgs("buy skin", text1, sprite, text2, actions, false, false, 0.5f);
        PopUpUI popup = PopUpManagerUI.instance.OpenImagePopUp(args);

        popup.image.color = skn.identificationColor;
        if (balance < skn.price) popup.buttons[0].interactable = false;


        //groupSelectionPanelsDict[selectedSlot].selectedSkin.owned = true; //just for testing
        UpdateDetailsPanel();
        UpdateSkinGroupButtons();

    }

    private async void BuyWithIngameWallet(string invoice)
    {

        try
        {
            await PlayerServiceConnections.instance.lnd.PayInvoice(invoice);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }
        PopUpArgs args = new PopUpArgs("info", "payment successfull");
        PopUpManagerUI.instance.OpenPopUp(args);
        Refresh();
    }
    private async void BuyWithExternalWallet(string invoice)
    {
        //image
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(invoice, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D tex = qrCode.GetGraphic(1);
        Color[] pixels = tex.GetPixels();
        for(int i = 0; i< pixels.Length; i++)
        {
            if (pixels[i].r > 0.5f){
                pixels[i] = Color.clear;
            }
            else
            {
                pixels[i] = Color.white;
            }
        }
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);

        //copy
        PopUpButtonArgs copyAction = new PopUpButtonArgs("copy invoice", () => Utility.CopyToClipboard(invoice),false);

        ImagePopUpArgs args = new ImagePopUpArgs("Lightning Payment", "", sprite, "", new List<PopUpButtonArgs> { copyAction },true, true);
        PopUpUI popup = PopUpManagerUI.instance.OpenImagePopUp(args);

        try
        {
            await PlayerServiceConnections.instance.BackendPlayerClient.WaitForPayment(invoice,60);
            if (popup != null) popup.Close();
            PopUpArgs args1 = new PopUpArgs("info", "payment successfull");
            PopUpManagerUI.instance.OpenPopUp(args1);
            Refresh();

        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            if (popup != null) popup.Close();
            PopUpArgs args1 = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(args1);
        }

    }

    private async void Equip()
    {
        // Todo check for errors
        try
        {
            await PlayerServiceConnections.instance.BackendPlayerClient.EquipSkin(selectedSkin.ID);
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
        selectedSkin = group.skins[0];

        if (selectedSkinGroupButton != null) selectedSkinGroupButton.SetSelection(false);
        selectedSkinGroupButton = skinGroupButtons.Find(o => o.group == group);
        selectedSkinGroupButton.SetSelection(true);

        UpdateDetailsPanel();
        UpdateSkinGroupButtons();
    }

    public void selectSkin(Skin skin)
    {
        selectedSkin = skin;
        UpdateDetailsPanel();
    }

}

