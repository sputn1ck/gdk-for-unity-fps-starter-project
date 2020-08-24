using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkinColorButtonUI : MonoBehaviour
{
    public Button button;
    public Image image;
    public GameObject frame;
    public GameObject underLine;
    public Image lockedImage;
    [HideInInspector]public SkinItem item;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public class SkinItemEvent : UnityEvent<SkinItem> { }
    public SkinItemEvent onClick = new SkinItemEvent();

    void OnClick()
    {
        onClick.Invoke(item);
    }
}
