using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;

public class CharacterMenuUI : MonoBehaviour
{
    public TextMeshProUGUI detailsHeaderText;
    public TextMeshProUGUI buyStateText;
    public Image detailsPreviewImage;
    public Button BuyAndEquipButton;
    public TextMeshProUGUI BuyAndEquipButtonText;

    public GameObject ColorButtonsContainer;
    List<SkinColorButtonUI> skinColorButtons;
    public SkinColorButtonUI skinColorButtonPrefab;

    public SkinGroupButtonUI skinGroupButtonPrefab;

    public List<SkinGroupSelectionPanel> groupSelectionPanels;
    Dictionary<SkinSlot, SkinGroupSelectionPanel> groupSelectionPanelsDict;


    public SkinsLibrary skinsLibrary;

    SkinSlot selectedSlot = SkinSlot.MASK;


    private void Awake()
    {
        skinsLibrary = Instantiate(skinsLibrary);
        skinsLibrary.Init();

    }
    private void Start()
    {
        skinColorButtons = ColorButtonsContainer.GetComponentsInChildren<SkinColorButtonUI>(true).ToList();
        groupSelectionPanelsDict = new Dictionary<SkinSlot, SkinGroupSelectionPanel>();

        foreach(SkinGroupSelectionPanel sgsp in groupSelectionPanels)
        {
            groupSelectionPanelsDict[sgsp.slot] = sgsp;
            sgsp.buttons = sgsp.container.GetComponentsInChildren<SkinGroupButtonUI>(true).ToList();
            sgsp.selectedSkin = skinsLibrary.GetSkin(PlayerPrefs.GetString("EquippedSkinID_" + sgsp.slot, skinsLibrary.settings[sgsp.slot].defaultSkinID));
            sgsp.subMenu.onActivate.AddListener(delegate{ selectedSlot = sgsp.slot; });
            
        }
        UpdateSelectionPanels();
    }

    void UpdateSkinGroupButtons(SkinGroupSelectionPanel panel)
    {
        List<SkinGroup> groups = skinsLibrary.settings[panel.slot];
        SkinGroup equippedGroup = skinsLibrary.GetGroup(PlayerPrefs.GetString("EquippedSkinID_" + panel.slot, skinsLibrary.settings[panel.slot].defaultSkinID));
        SkinGroup selectedGroup = skinsLibrary.GetGroup(panel.selectedSkin.ID);
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
        detailsHeaderText.text = group.name;
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
                BuyAndEquipButton.onClick.AddListener(equip);
            }
        }
        else
        {
            buyStateText.text = Utility.SatsToShortString(skin.price, UITinter.tintDict[TintColor.Sats]);
            BuyAndEquipButtonText.text = "buy";
            BuyAndEquipButton.onClick.RemoveAllListeners();
            BuyAndEquipButton.onClick.AddListener(buy);
        }
        UpdateSkinGroupColors(group);
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

            skinColorButtons[i].gameObject.SetActive(true);

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
            scb.image.color = skn.identificationColor;
            scb.skin = skn;

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
        return PlayerPrefs.GetString("EquippedSkinID_"+slot,skinsLibrary.settings[slot].defaultSkinID);
    }

    Skin GetEquippedSkin(SkinSlot slot)
    {
        string id = GetEquippedSkinID(slot);
        return skinsLibrary.GetSkin(id);
    }


    private void buy()
    {
        groupSelectionPanelsDict[selectedSlot].selectedSkin.owned = true; //just for testing
        UpdateDetailsPanel();
        UpdateSelectionPanels();
    }

    private void equip()
    {
        PlayerPrefs.SetString("EquippedSkinID_"+skinsLibrary.GetGroup(groupSelectionPanelsDict[selectedSlot].selectedSkin.ID).slot, groupSelectionPanelsDict[selectedSlot].selectedSkin.ID);
        PlayerPrefs.Save();
        UpdateDetailsPanel();
        UpdateSelectionPanels();
    }
    public void UpdateDetailsPanel()
    {
        SkinGroup g = skinsLibrary.GetGroup(groupSelectionPanelsDict[selectedSlot].selectedSkin.ID);
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
        UpdateDetailsPanel(skinsLibrary.GetGroup(skin.ID),skin);
    }

    void UpdateSelectionPanels()
    {
        foreach (SkinGroupSelectionPanel p in groupSelectionPanels)
        {
            UpdateSkinGroupButtons(p);
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

