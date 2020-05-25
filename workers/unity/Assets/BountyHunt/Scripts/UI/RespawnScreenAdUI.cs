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
    static List<string> urls;
    Advertiser advertiser;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Set(Advertiser advertiser)
    {
        if (advertiser == null) gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(false);
        }

        this.advertiser = advertiser;

        if (!urls.Contains(advertiser.url))
        {
            text.GetComponent<UITinter>().updateColor(TintColor.Link);
            button.interactable = true;
        }
        else{
            text.GetComponent<UITinter>().updateColor(TintColor.Primary);
            button.interactable = false;
        }
        image.material = advertiser.GetRandomMaterial(Advertiser.AdMaterialType.SQUARE);

    }

    void OnClick()
    {
        text.GetComponent<UITinter>().updateColor(TintColor.Primary);
        ClientAdManagerBehaviour.instance.AddUrl(advertiser.url);
        button.interactable = false;
    }

    public static void OpenAllLinks()
    {
        foreach(string link in urls)
        {
            Application.OpenURL(link);
        }
        urls.Clear();
    }
}
