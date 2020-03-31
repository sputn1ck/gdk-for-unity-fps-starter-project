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
    public Skin skin;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public class skinEvent : UnityEvent<Skin> { }
    public skinEvent onClick = new skinEvent();

    void OnClick()
    {
        onClick.Invoke(skin);
    }
}
