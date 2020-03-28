using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

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

    public SkinsLibrary skinsLibrary;

    Skin selectedSkin;

    private void Start()
    {
        skinColorButtons = ColorButtonsContainer.GetComponentsInChildren<SkinColorButtonUI>(true).ToList();
    }

    public void UpdateDetailsPanel(SkinGroup group, Skin skin)
    {
        selectedSkin = skin;
        detailsHeaderText.text = group.name;
        detailsPreviewImage.sprite = group.sprite;
        BuyAndEquipButton.interactable = true;

        if (skin.owned)
        {
            buyStateText.text = "owned";
            if (skin == GetEquippedSkin())
            {
                BuyAndEquipButtonText.text = "equipped";
                BuyAndEquipButton.gameObject.SetActive(false);
            }
            else
            {
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
            BuyAndEquipButton.onClick.AddListener(equip);
        }
    }

    public void UpdateSkinGroupColors(SkinGroup group)
    {
        Skin equipped = GetEquippedSkin();
        
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

            if (equipped == skn) scb.frame.SetActive(true);
            else scb.frame.SetActive(false);

            if (selectedSkin == skn) scb.underLine.SetActive(false);

        }
    }

    string GetEquippedSkinID()
    {
        return PlayerPrefs.GetString("EquippedSkinID","");
    }

    Skin GetEquippedSkin()
    {
        string id = GetEquippedSkinID();
        return skinsLibrary.getSkin(id);
    }


    private void buy()
    {
        selectedSkin.owned = true; //just for testing
    }

    private void equip()
    {

    }

    public void UpdateDetailsPanel(SkinGroup group)
    {
        
        UpdateDetailsPanel(group, group.skins[0]);
    }
}

