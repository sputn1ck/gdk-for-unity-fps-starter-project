using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RespawnScreenAdUI : MonoBehaviour
{
    public RawImage image;
    public Button button;
    public TextMeshProUGUI text;
    Advertiser advertiser;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Set(Advertiser advertiser)
    {
        if (advertiser == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        this.advertiser = advertiser;
        if (!UrlMemory.UrlInQueue(advertiser.url))
        {
            text.GetComponent<UITinter>().updateColor(TintColor.Link);
            text.text = advertiser.url;
            button.interactable = true;
        }
        else{
            text.GetComponent<UITinter>().updateColor(TintColor.Primary);
            text.text = GameText.AdOpenInfo;
            button.interactable = false;
        }
        image.texture = advertiser.GetRandomTexture(Advertiser.AdMaterialType.SQUARE);

    }

    void OnClick()
    {
        text.GetComponent<UITinter>().updateColor(TintColor.Primary);
        text.text = GameText.AdOpenInfo;
        UrlMemory.AddUrl(advertiser.url);
        button.interactable = false;
    }

}
