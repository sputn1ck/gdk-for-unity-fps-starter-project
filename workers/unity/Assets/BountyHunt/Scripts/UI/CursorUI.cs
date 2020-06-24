using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CursorUI : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite onClickSprite;
    public Camera cam;
    Image image;
    [SerializeField]TextMeshProUGUI toolTipText;

    public static CursorUI Instance;

    private void Awake()
    {
        image = GetComponent<Image>();
        Instance = this;
        Cursor.visible = false;
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        image.sprite = Input.GetMouseButton(0) ? onClickSprite : defaultSprite;
    }

    public void SetToolTipText(string text)
    {
        toolTipText.text = text;
    }

    public void RemoveToolTipText()
    {
        toolTipText.text = "";
    }
}
