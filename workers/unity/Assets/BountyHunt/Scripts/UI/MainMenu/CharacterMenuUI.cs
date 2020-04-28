using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;
using Daemon;

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

    public List<SkinGroupSelectionPanel> groupSelectionPanels;
    Dictionary<SkinSlot, SkinGroupSelectionPanel> groupSelectionPanelsDict;

    SkinSlot selectedSlot = SkinSlot.BODY;
    SkinsLibrary playerSkinsLibrary;

    private void Start()
    {
        refreshButton.onClick.AddListener(Init);
        ClientEvents.instance.onServicesSetup.AddListener(Init);
    }

    async void Init()
    {
        Debug.Log("initializing characterMenu");
        
        //Todo get from backend instead
        var shopSkins = await PlayerServiceConnections.instance.BackendPlayerClient.GetAllSkins();
        var testOwnedIDs = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInventory();
        //List<string> testIDs = new List<string> { "robot_default", "robot_gold", "robot_6", "hs_default","robot_3","robot_bronze" };
        //List<string> testOwnedIDs = new List<string> { "robot_default", "robot_6", "hs_default", "robot_bronze" };
        playerSkinsLibrary = Instantiate(SkinsLibrary.MasterInstance);
        playerSkinsLibrary.InitializeForCharacterMenu(shopSkins, testOwnedIDs.OwnedSkins.ToArray());
        //skinsLibrary.Initialize();

        skinColorButtons = ColorButtonsContainer.GetComponentsInChildren<SkinColorButtonUI>(true).ToList();
        groupSelectionPanelsDict = new Dictionary<SkinSlot, SkinGroupSelectionPanel>();

        foreach (SkinGroupSelectionPanel sgsp in groupSelectionPanels)
        {
            groupSelectionPanelsDict[sgsp.slot] = sgsp;
            sgsp.buttons = sgsp.container.GetComponentsInChildren<SkinGroupButtonUI>(true).ToList();
            sgsp.selectedSkin = playerSkinsLibrary.GetSkin(PlayerPrefs.GetString("EquippedSkinID_" + sgsp.slot, playerSkinsLibrary.skinSlotSettings[sgsp.slot].defaultSkinID), sgsp.slot);
            sgsp.subMenu.onActivate.AddListener(delegate { selectedSlot = sgsp.slot; });
            
        }
        UpdateSelectionPanels();
        UpdateDetailsPanel();
    }

    void UpdateSkinGroupButtons(SkinGroupSelectionPanel panel)
    {
        List<SkinGroup> groups = playerSkinsLibrary.skinSlotSettings[panel.slot];
        SkinGroup equippedGroup = playerSkinsLibrary.GetGroup(PlayerPrefs.GetString("EquippedSkinID_" + panel.slot, playerSkinsLibrary.skinSlotSettings[panel.slot].defaultSkinID),panel.slot);
        SkinGroup selectedGroup = playerSkinsLibrary.GetGroup(panel.selectedSkin.ID,panel.slot);
        int counter = 0;
        foreach (SkinGroupButtonUI sgb in panel.buttons)
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
            SkinGroupButtonUI sgb = Instantiate(skinGroupButtonPrefab, panel.container);
            sgb.gameObject.SetActive(true);
            sgb.set(groups[i], SelectSkinGroup);

            if (groups[i] == equippedGroup) sgb.SetEquippedState(true);
            else sgb.SetEquippedState(false);

            if (groups[i] == selectedGroup) sgb.SetSelection(true);
            else sgb.SetSelection(false);
        }
    }

    public void UpdateDetailsPanel(SkinGroup group, Skin skin)
    {
        groupSelectionPanelsDict[group.slot].selectedSkin = skin;
        detailsHeaderText.text = group.groupName;
        detailsPreviewImage.sprite = group.sprite;
        BuyAndEquipButton.interactable = true;

        BuyAndEquipButton.gameObject.SetActive(true);

        if (skin.owned)
        {
            
            if (skin == GetEquippedSkin(group.slot))
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
        UpdateSkinGroupColors(group);
        PreviewSpot.Instance.SetSkin(skin);
    }

    public void UpdateSkinGroupColors(SkinGroup group)
    {
        Skin equipped = GetEquippedSkin(group.slot);
        
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

            if (equipped == skn) scb.frame.SetActive(true);
            else scb.frame.SetActive(false);

            if (groupSelectionPanelsDict[group.slot].selectedSkin == skn) scb.underLine.SetActive(true);
            else scb.underLine.SetActive(false);

            scb.onClick.RemoveAllListeners();
            scb.onClick.AddListener(selectSkin);
        }
    }

    string GetEquippedSkinID(SkinSlot slot)
    {
        return PlayerPrefs.GetString("EquippedSkinID_"+slot, playerSkinsLibrary.skinSlotSettings[slot].defaultSkinID);
    }

    Skin GetEquippedSkin(SkinSlot slot)
    {
        string id = GetEquippedSkinID(slot);
        return playerSkinsLibrary.GetSkin(id,slot);
    }


    private async void Buy()
    {
        Skin skn = groupSelectionPanelsDict[selectedSlot].selectedSkin;
        SkinGroup grp = groupSelectionPanelsDict[selectedSlot].selectedGroup.group;
        // Todo popup
        try
        {
            var res = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInvoice(skn.ID);

            GetBalanceResponse balanceResponse = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
            long balance = balanceResponse.DaemonBalance;

            string text1 = "You are going to buy: \n " + grp.groupName +" \n for "+ Utility.SatsToShortString(skn.price,UITinter.tintDict[TintColor.Sats]);
            Sprite sprite = groupSelectionPanelsDict[selectedSlot].selectedGroup.group.sprite;
            string text2 = "";
            if (balance < skn.price) text2 = "Your Ingame Wallet doesent cover the required amount!"; //Todo hint when there is no channel, or to less balance
            List<LabelAndAction> actions = new List<LabelAndAction>();
            actions.Add(new LabelAndAction("ingame Wallet", BuyWithIngameWallet));
            actions.Add(new LabelAndAction("external Wallet", BuyWithExternalWallet));

            ImagePopUpArgs args = new ImagePopUpArgs("buy skin", text1, sprite, text2, actions, false, false, 0.5f);
            PopUpUI popup = PopUpManagerUI.instance.OpenImagePopUp(args);

            popup.image.color = skn.identificationColor;
            if (balance < skn.price) popup.buttons[0].interactable = false;
        }
        catch(Exception e)
        {
            PopUpArgs args = new PopUpArgs("Error",e.Message);
            PopUpManagerUI.instance.OpenPopUp(args);
        }
        //groupSelectionPanelsDict[selectedSlot].selectedSkin.owned = true; //just for testing
        UpdateDetailsPanel();
        UpdateSelectionPanels();
    }

    private async void BuyWithIngameWallet()
    {

    }
    private async void BuyWithExternalWallet()
    {

    }

    private void Equip()
    {
        // Todo check for errors
        PlayerServiceConnections.instance.BackendPlayerClient.EquipSkin(groupSelectionPanelsDict[selectedSlot].selectedSkin.ID);
        PlayerPrefs.SetString("EquippedSkinID_"+selectedSlot, groupSelectionPanelsDict[selectedSlot].selectedSkin.ID);
        PlayerPrefs.Save();
        UpdateDetailsPanel();
        UpdateSelectionPanels();
    }
    public void UpdateDetailsPanel()
    {
        SkinGroup g = playerSkinsLibrary.GetGroup(groupSelectionPanelsDict[selectedSlot].selectedSkin.ID,selectedSlot);
        Skin s = groupSelectionPanelsDict[selectedSlot].selectedSkin;
        UpdateDetailsPanel(g,s);
    }
    public void UpdateDetailsPanel(SkinGroup group)
    {
        
        UpdateDetailsPanel(group, group.skins[0]);
    }

    public void SelectSkinGroup(SkinGroup group)
    {
        UpdateDetailsPanel(group);
        SkinGroupSelectionPanel panel = groupSelectionPanelsDict[group.slot];

        if (panel.selectedGroup) panel.selectedGroup.SetSelection(false);
        panel.selectedGroup = panel.buttons.Find(o => o.group == group);
        panel.buttons.Find(o => o.group == group).SetSelection(true);
        UpdateSelectionPanels();
        

    }

    public void selectSkin(Skin skin)
    {
        UpdateDetailsPanel(playerSkinsLibrary.GetGroup(skin.ID,selectedSlot),skin);
    }

    void UpdateSelectionPanels()
    {
        foreach (SkinGroupSelectionPanel p in groupSelectionPanels)
        {
            UpdateSkinGroupButtons(p);
        }
    }

    void ShowEquippedSkins()
    {
        foreach( var set in playerSkinsLibrary.skinSlotSettings)
        {
            Skin skin = playerSkinsLibrary.GetSkin(PlayerPrefs.GetString("EquippedSkinID_" + set.Value.slot), set.Value.slot);
            PreviewSpot.Instance.SetSkin(skin);
        }
    }
    
}

[System.Serializable]
public class SkinGroupSelectionPanel
{
    public SkinSlot slot;
    public Transform container;
    [HideInInspector]public List<SkinGroupButtonUI> buttons;
    [HideInInspector]public SkinGroupButtonUI selectedGroup;
    [HideInInspector] public Skin selectedSkin;
    public SlideSubMenuUI subMenu;
}

