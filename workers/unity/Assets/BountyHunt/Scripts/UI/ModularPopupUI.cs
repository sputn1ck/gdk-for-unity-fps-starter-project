using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class ModularPopUpUI : MonoBehaviour
{
    public TextMeshProUGUI headlineText;
    public Button xButton;
    public Transform ElementsContainer;
    public List<PopUpElementPrefabInfo> prefabs;

    UnityAction closeAction;

    public void SetUp(string headline, bool showX, List<IPopupElement> elements, UnityAction closeAction = null)
    {
        headlineText.text = headline;
        if (showX) xButton.onClick.AddListener(Close);
        else xButton.gameObject.SetActive(showX);

        foreach (IPopupElement element in elements)
        {
            var epi = prefabs.Find(e => e.key == element.GetKey());
            GameObject obj = Instantiate(epi.prefab, ElementsContainer);
            element.SetUp(obj,this);
        }

    }

    public void Close()
    {
        closeAction?.Invoke();
        Destroy(gameObject);
    }
}
[System.Serializable] public struct PopUpElementPrefabInfo
{
    public string key;
    public GameObject prefab;
}

 
public interface IPopupElement
{
    string GetKey();
    void SetUp(GameObject obj, ModularPopUpUI popup);
}

public class TextPopupElement : IPopupElement
{
    public string text;

    public string GetKey()
    {
        return "text";
    }

    public void SetUp(GameObject obj, ModularPopUpUI popup)
    {
        obj.GetComponent<TextMeshProUGUI>().text = text;
    }
}

public class ButtonPopupElement : IPopupElement
{
    public PopUpButtonArgs args;

    public string GetKey()
    {
        return "button";
    }

    public void SetUp(GameObject obj, ModularPopUpUI popup)
    {
        
        Button b = obj.GetComponent<Button>();
        b.GetComponentInChildren<TextMeshProUGUI>().text = args.label;
        b.onClick.AddListener(args.action);
        if (args.closePopupOnClick) b.onClick.AddListener(popup.Close);
    }
}

public class horizontalButtonListPopupElement : IPopupElement
{
    public List<PopUpButtonArgs> buttonArgs;

    public string GetKey()
    {
        return "horizontalButtonList";
    }

    public void SetUp(GameObject obj, ModularPopUpUI popup)
    {
        PopUpButtonListUI buttonList = obj.GetComponent<PopUpButtonListUI>();

        foreach (var args in buttonArgs)
        {
            Button b = GameObject.Instantiate(buttonList.buttonPrefab, buttonList.buttonContainer);

            b.GetComponentInChildren<TextMeshProUGUI>().text = args.label;
            b.onClick.AddListener(args.action);
            if (args.closePopupOnClick) b.onClick.AddListener(popup.Close);
        }
    }
}

public class VerticalButtonListPopupElement : IPopupElement
{
    public List<PopUpButtonArgs> buttonArgs;

    public string GetKey()
    {
        return "verticalButtonList";
    }

    public void SetUp(GameObject obj, ModularPopUpUI popup)
    {
        PopUpButtonListUI buttonList = obj.GetComponent<PopUpButtonListUI>();

        foreach (var args in buttonArgs)
        {
            Button b = GameObject.Instantiate(buttonList.buttonPrefab, buttonList.buttonContainer);

            b.GetComponentInChildren<TextMeshProUGUI>().text = args.label;
            b.onClick.AddListener(args.action);
            if (args.closePopupOnClick) b.onClick.AddListener(popup.Close);
        }
    }
}

public class ImagePopupElement : IPopupElement
{
    public Sprite sprite;
    public TintColor tint = TintColor.None;
    public Color color = Color.white;
    public float scale = 1;

    public string GetKey()
    {
        return "image";
    }

    public void SetUp(GameObject obj, ModularPopUpUI popup)
    {
        Image image = obj.GetComponent<Image>();
        UITinter tinter = image.GetComponent<UITinter>();
        LayoutElement layout = obj.GetComponent<LayoutElement>();

        image.sprite = sprite;

        if (tint != TintColor.None) tinter.updateColor(tint);
        else image.color = color;

        layout.preferredHeight *= scale;
    }
}

public class InputfieldPopupElement : IPopupElement
{
    public string preInputFieldText = "";
    public string postInputFieldText = "";
    public UnityAction<string> onValueChangeAction = null;
    public TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard;
    public TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft;
    public string defaultInputText = "";
    public string placeholderText = "";

    public string GetKey()
    {
        return "inputField";
    }

    public void SetUp(GameObject obj, ModularPopUpUI popup)
    {
        PopUpInputFieldUI pif = obj.GetComponent<PopUpInputFieldUI>();
        
        if (preInputFieldText != "") pif.preInputFieldText.text = preInputFieldText;
        else pif.preInputFieldText.gameObject.SetActive(false);
        if (postInputFieldText != "") pif.postInputFieldText.text = postInputFieldText;
        else pif.postInputFieldText.gameObject.SetActive(false);

        pif.inputField.onValueChanged.AddListener(onValueChangeAction);

        pif.inputField.contentType = contentType;

        pif.inputField.text = defaultInputText;
        (pif.inputField.placeholder as TextMeshProUGUI).text = placeholderText;

        pif.inputField.textComponent.alignment = alignment;
        (pif.inputField.placeholder as TextMeshProUGUI).alignment = alignment;

    }
}

public class PlayerSatsSettingsPopupElement : IPopupElement
{
    public long defaultSats = 1000;
    public float playersatsPrice = 2;
    public string priceLabelText = "price:";
    public UnityAction<long> ingamePayAction;
    public UnityAction<long> walletPayAction;

    long price;
    PopUpPlayerSatsSettingsUI ppss;

    public string GetKey()
    {
        return "playerSatsSettings";
    }

    public void SetUp(GameObject obj, ModularPopUpUI popup)
    {
        ppss = obj.GetComponent<PopUpPlayerSatsSettingsUI>();

        ppss.inputField.text = defaultSats.ToString();
        UpdatePrice(defaultSats.ToString());
        ppss.inputField.onValueChanged.AddListener(UpdatePrice);

        ppss.PriceLabelText.text = priceLabelText;

        ppss.IngamePayButton.onClick.AddListener(OnIngameButtonPress);
        ppss.WalletPayButton.onClick.AddListener(OnWalletButtonPress);
        ppss.IngamePayButton.onClick.AddListener(popup.Close);
        ppss.WalletPayButton.onClick.AddListener(popup.Close);

        ppss.inputField.StartCoroutine(ForceRefresh(ppss.inputField));
    }

    void UpdatePrice(string playerSats)
    {
        long sats = long.Parse(playerSats);
        price = (long)(sats * playersatsPrice);
        ppss.PriceText.text = price.ToString();
    }

    void OnIngameButtonPress()
    {
        ingamePayAction.Invoke(price);
    }

    void OnWalletButtonPress()
    {
        walletPayAction.Invoke(price);
    }

    IEnumerator ForceRefresh(TMP_InputField input)
    {
        yield return new WaitForEndOfFrame();
        input.textComponent.ForceMeshUpdate();
    }
}
