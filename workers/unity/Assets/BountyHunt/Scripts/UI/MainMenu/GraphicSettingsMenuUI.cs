using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class GraphicSettingsMenuUI : MonoBehaviour
{
    public Button resolutionButton;

    public Button qualityButton;

    public Button displayModeButton;

    private void Start()
    {
        resolutionButton.onClick.AddListener(OnResolutionButtonClick);
        qualityButton.onClick.AddListener(OnQualityButtonClick);
        displayModeButton.onClick.AddListener(OnDisplayModeButtonClick);

        resolutionButton.GetComponentInChildren<TextMeshProUGUI>().text = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        qualityButton.GetComponentInChildren<TextMeshProUGUI>().text = ((GraphicsQuality)QualitySettings.GetQualityLevel()).ToString();
        displayModeButton.GetComponentInChildren<TextMeshProUGUI>().text = ((DisplayMode)Screen.fullScreenMode).ToString();

        resolutionButton.GetComponentInChildren<TextSizer>().Refresh();
        qualityButton.GetComponentInChildren<TextSizer>().Refresh();
        displayModeButton.GetComponentInChildren<TextSizer>().Refresh();


    }

    //resolution
    void OnResolutionButtonClick()
    {
        List<LabelAndAction> resos = new List<LabelAndAction>();
        foreach(Resolution res in Screen.resolutions)
        {
            resos.Add(resolutionLabelAndAction(res));
        }
        PopUpEventArgs args = new PopUpEventArgs("Resolution", "", resos, true);
        ClientEvents.instance.onPopUp.Invoke(args);
    }

    LabelAndAction resolutionLabelAndAction(Resolution res)
    {
        string label = res.width + " x " + res.height;
        UnityAction action = delegate { SetResolution(res); };
        return new LabelAndAction(label, action);
    }

    void SetResolution(Resolution res)
    {
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        resolutionButton.GetComponentInChildren<TextMeshProUGUI>().text = res.width + " x " + res.height;
        resolutionButton.GetComponentInChildren<TextSizer>().Refresh();
    }

    //quality
    void OnQualityButtonClick()
    {
        List<LabelAndAction> quals = new List<LabelAndAction>();
        foreach (GraphicsQuality qual in (GraphicsQuality[])Enum.GetValues(typeof(GraphicsQuality)))
        {
            quals.Add(QualityLabelAndAction(qual));
        }
        PopUpEventArgs args = new PopUpEventArgs("Quality", "", quals, true);
        ClientEvents.instance.onPopUp.Invoke(args);
    }
    void SetQuality(GraphicsQuality quality)
    {
        QualitySettings.SetQualityLevel((int)quality);
        qualityButton.GetComponentInChildren<TextMeshProUGUI>().text = quality.ToString();
        qualityButton.GetComponentInChildren<TextSizer>().Refresh();
    }
    public enum GraphicsQuality
    {
        low = 0,
        medium = 1,
        high = 2
    }
    LabelAndAction QualityLabelAndAction(GraphicsQuality quality)
    {
        string label = quality.ToString();
        UnityAction action = delegate { SetQuality(quality); };
        return new LabelAndAction(label, action);
    }

    //display mode
    void OnDisplayModeButtonClick()
    {
        List<LabelAndAction> modes = new List<LabelAndAction>();
        foreach (DisplayMode mode in (DisplayMode[])Enum.GetValues(typeof(DisplayMode)))
        {
            modes.Add(DisplayModeLabelAndAction(mode));
        }
        PopUpEventArgs args = new PopUpEventArgs("Display Mode", "", modes, true);
        ClientEvents.instance.onPopUp.Invoke(args);
    }
    void SetDisplayMode(DisplayMode mode)
    {
        Screen.fullScreenMode = (FullScreenMode)mode;
        displayModeButton.GetComponentInChildren<TextMeshProUGUI>().text = mode.ToString();
        displayModeButton.GetComponentInChildren<TextSizer>().Refresh();
    }
    public enum DisplayMode
    {
        fullscreen = 1,
        windowed = 3
    }
    LabelAndAction DisplayModeLabelAndAction(DisplayMode mode)
    {
        string label = mode.ToString();
        UnityAction action = delegate { SetDisplayMode(mode); };
        return new LabelAndAction(label, action);
    }


}
