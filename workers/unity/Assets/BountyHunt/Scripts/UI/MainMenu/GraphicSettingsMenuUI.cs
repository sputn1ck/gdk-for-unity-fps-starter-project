using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class GraphicSettingsMenuUI : MonoBehaviour
{
    public Button resolutionButton;

    public Button qualityButton;

    public Button displayModeButton;

    public Button frameRateButton;

    Resolution targetResolution;

    private void Start()
    {
        resolutionButton.onClick.AddListener(OnResolutionButtonClick);
        qualityButton.onClick.AddListener(OnQualityButtonClick);
        displayModeButton.onClick.AddListener(OnDisplayModeButtonClick);
        frameRateButton.onClick.AddListener(OnFramerateButtonClick);

        resolutionButton.GetComponentInChildren<TextMeshProUGUI>().text = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        qualityButton.GetComponentInChildren<TextMeshProUGUI>().text = ((GraphicsQuality)QualitySettings.GetQualityLevel()).ToString();
        displayModeButton.GetComponentInChildren<TextMeshProUGUI>().text = ((DisplayMode)Screen.fullScreenMode).ToString();
        frameRateButton.GetComponentInChildren<TextMeshProUGUI>().text = Screen.currentResolution.refreshRate + " Hz";

        resolutionButton.GetComponentInChildren<TextSizer>().Refresh();
        qualityButton.GetComponentInChildren<TextSizer>().Refresh();
        displayModeButton.GetComponentInChildren<TextSizer>().Refresh();
        frameRateButton.GetComponentInChildren<TextSizer>().Refresh();

        targetResolution = Screen.currentResolution;

    }

    //resolution
    void OnResolutionButtonClick()
    {
        List<PopUpButtonArgs> resos = new List<PopUpButtonArgs>();

        var resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct();
        foreach (Resolution res in resolutions)
        {
            resos.Add(resolutionLabelAndAction(res));
        }
        PopUpArgs args = new PopUpArgs("Resolution", "", resos, true);
        PopUpManagerUI.instance.OpenPopUp(args);
    }

    PopUpButtonArgs resolutionLabelAndAction(Resolution res)
    {
        string label = res.width + " x " + res.height;
        UnityAction action = delegate { SetResolution(res); };
        return new PopUpButtonArgs(label, action);
    }

    void SetResolution(Resolution res)
    {
        int fps = Screen.currentResolution.refreshRate;
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode,fps);
        resolutionButton.GetComponentInChildren<TextMeshProUGUI>().text = res.width + " x " + res.height;
        frameRateButton.GetComponentInChildren<TextMeshProUGUI>().text = Screen.currentResolution.refreshRate + " Hz";
        resolutionButton.GetComponentInChildren<TextSizer>().Refresh();
        frameRateButton.GetComponentInChildren<TextSizer>().Refresh();
        targetResolution.width = res.width;
        targetResolution.height = res.height;
    }

    void OnFramerateButtonClick()
    {
        List<PopUpButtonArgs> resos = new List<PopUpButtonArgs>();
        List<Resolution> listedRes = new List<Resolution>();

        var framerates = Screen.resolutions.Select(resolution => resolution.refreshRate).Distinct();
        foreach (int fr in framerates)
        {
            resos.Add(framerateLabelAndAction(fr));
        }
        PopUpArgs args = new PopUpArgs("Framerate", "", resos, true);
        PopUpManagerUI.instance.OpenPopUp(args);
    }

    PopUpButtonArgs framerateLabelAndAction(int framerate)
    {
        string label = framerate + " Hz";
        UnityAction action = delegate { SetFramerate(framerate); };
        return new PopUpButtonArgs(label, action);
    }

    void SetFramerate(int framerate)
    {
        Screen.SetResolution(targetResolution.width,targetResolution.height,Screen.fullScreenMode, framerate);
        frameRateButton.GetComponentInChildren<TextMeshProUGUI>().text = framerate + " Hz";
        frameRateButton.GetComponentInChildren<TextSizer>().Refresh();
        targetResolution.refreshRate = framerate;
    }

    //quality
    void OnQualityButtonClick()
    {
        List<PopUpButtonArgs> quals = new List<PopUpButtonArgs>();
        foreach (GraphicsQuality qual in (GraphicsQuality[])Enum.GetValues(typeof(GraphicsQuality)))
        {
            quals.Add(QualityLabelAndAction(qual));
        }
        PopUpArgs args = new PopUpArgs("Quality", "", quals, true);
        PopUpManagerUI.instance.OpenPopUp(args);
    }
    void SetQuality(GraphicsQuality quality)
    {
        QualitySettings.SetQualityLevel((int)quality);
        qualityButton.GetComponentInChildren<TextMeshProUGUI>().text = quality.ToString();
        qualityButton.GetComponentInChildren<TextSizer>().Refresh();
    }
    public enum GraphicsQuality
    {
        low = 2,
        medium = 1,
        high = 0
    }
    PopUpButtonArgs QualityLabelAndAction(GraphicsQuality quality)
    {
        string label = quality.ToString();
        UnityAction action = delegate { SetQuality(quality); };
        return new PopUpButtonArgs(label, action);
    }

    //display mode
    void OnDisplayModeButtonClick()
    {
        List<PopUpButtonArgs> modes = new List<PopUpButtonArgs>();
        foreach (DisplayMode mode in (DisplayMode[])Enum.GetValues(typeof(DisplayMode)))
        {
            modes.Add(DisplayModeLabelAndAction(mode));
        }
        PopUpArgs args = new PopUpArgs("Display Mode", "", modes, true);
        PopUpManagerUI.instance.OpenPopUp(args);
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
    PopUpButtonArgs DisplayModeLabelAndAction(DisplayMode mode)
    {
        string label = mode.ToString();
        UnityAction action = delegate { SetDisplayMode(mode); };
        return new PopUpButtonArgs(label, action);
    }


}
