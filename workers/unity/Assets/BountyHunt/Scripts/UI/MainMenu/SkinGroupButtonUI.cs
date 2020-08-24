using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Events;

public class SkinGroupButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    public Image symbol;
    public GameObject frame;
    public GameObject underLine;
    public Transform colorfieldsContainer;

    Image[] colorfields;
    public SkinGroup group { get; private set;}

    public class SkinGroupEvent : UnityEvent<SkinGroup> { };
    public SkinGroupEvent onClick = new SkinGroupEvent();

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
        colorfields = colorfieldsContainer.GetComponentsInChildren<Image>();
    }

    public void set(SkinGroup group,UnityAction<SkinGroup> action)
    {
        onClick.RemoveAllListeners();
        onClick.AddListener(action);
        this.group = group;

        symbol.sprite = group.sprite;


        List<SkinItem> owned = new List<SkinItem>(SkinShop.ItemGroupDict[group].Where(o => o.owned));
        colorfieldsContainer.gameObject.SetActive(true);
        for (int i = 0; i<colorfields.Length;i++)
        {
            if (owned.Count > i)
            {

                colorfields[i].color = owned[i].skin.identificationColor;
            }
            else
            {
                if (i == 0)
                {
                    colorfieldsContainer.gameObject.SetActive(false);
                    return;
                }
                colorfields[i].color = Color.clear;
            }
        }
    }

    public void SetSelection (bool select)
    {
        underLine.gameObject.SetActive(select);
    }
    public void SetEquippedState(bool equipped)
    {
        frame.gameObject.SetActive(equipped);

    }

    void OnClick()
    {
        onClick.Invoke(group);
    }
}


