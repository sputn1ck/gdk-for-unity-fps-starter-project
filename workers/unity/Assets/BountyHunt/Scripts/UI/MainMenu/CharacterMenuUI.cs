using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterMenuUI : MonoBehaviour
{
    public TextMeshProUGUI detailsHeaderText;
    public TextMeshProUGUI buyStateText;
    public Image detailsPreviewImage;
    public List<SkinGroup> skinGroups;
    public Button BuyAndEquipButton;
    public TextMeshProUGUI BuyAndEquipButtonText;

    public SkinsLibrary skinsLirary;
    

    public void UpdateDetailsPanel(SkinGroup group, Skin skin)
    {
        
        detailsHeaderText.text = group.name;
        detailsPreviewImage.sprite = group.sprite;
        BuyAndEquipButton.interactable = true;

        if (skin.owned)
        {
            buyStateText.text = "owned";
            string equippedID = PlayerPrefs.GetString("EquippedSkinID");
            if (skin.ID == equippedID)
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
            buyStateText.text = Utility.SatsToShortString(skin.price,UITinter.tintDict[TintColor.Sats]);
            BuyAndEquipButtonText.text = "buy";
            BuyAndEquipButton.onClick.RemoveAllListeners();
            BuyAndEquipButton.onClick.AddListener(equip);
        }
    }

    private void buy()
    {

    }

    private void equip()
    {

    }

    public void UpdateDetailsPanel(SkinGroup group)
    {
        
        UpdateDetailsPanel(group, group.skins[0]);
    }
}

