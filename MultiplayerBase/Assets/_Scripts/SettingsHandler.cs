using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using NUnit.Framework;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    private GameObject rebindPrompt;
    private GameObject subMenus;
    private string currentRebind;
    private List<KeyCode> forbiddenKeys = new List<KeyCode>
    {
        // These keys are disallowed from being used for keybinds for various reasons
        KeyCode.Escape,
        KeyCode.LeftMeta,
        KeyCode.LeftWindows,
        KeyCode.RightMeta,
        KeyCode.RightWindows
    };

    public ClientSettings settings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rebindPrompt = settingsMenu.transform.Find("RebindPanel").gameObject;
        subMenus = settingsMenu.transform.Find("SubMenus").gameObject;

        // Attempt to load settings from file, if not found then use default settings
        try
        {
            string settingsFile = File.ReadAllText("Settings.data");
            settings = JsonUtility.FromJson<ClientSettings>(settingsFile);
        }
        catch (FileNotFoundException)
        {
            print("Settings file not found, using default settings");
            settings = new ClientSettings(false);
        }
    }

    void Update()
    {
        if (currentRebind == null)
        {
            // Temp debug things to make sure things are working, only runs when a rebind prompt isn't active
            if (Input.GetKey(settings.controls.debugFlash))
            {
               settingsMenu.transform.Find("TempText").gameObject.SetActive(true);
            }
            else
            {
                settingsMenu.transform.Find("TempText").gameObject.SetActive(false);
            }
        }
    }
    
    void OnGUI()
    {
        if (currentRebind != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if (!forbiddenKeys.Contains(e.keyCode)) // Checks if the key is allowed to be used for keybinds
                {
                    SetControl(currentRebind, e.keyCode);
                    currentRebind = null;
                    rebindPrompt.SetActive(false);
                }
            }
        }
    }

    public void OpenSubMenu(GameObject newSubMenu)
    {
        foreach (Transform subMenu in subMenus.transform)
        {
            subMenu.gameObject.SetActive(false);
        }

        newSubMenu.SetActive(true);
    }

    public void ReadSettings()
    {
        GameObject tempText = settingsMenu.transform.Find("TempText").gameObject;
        tempText.GetComponent<TextMeshProUGUI>().text = JsonUtility.ToJson(settings);

        // Loads accessibility settings
        Transform accessibilityList = subMenus.transform.Find("Accessibility");
        for (int i = 0; i < accessibilityList.childCount; i++)
        {
            Transform child = accessibilityList.GetChild(i);

            if (child.gameObject.CompareTag("CheckboxSetting"))
            {
                child.Find("Checkbox").Find("Checkmark").gameObject.SetActive(GetToggleValue(child.gameObject.name));
            }
        }

        // Loads control settings
        Transform controlsList = subMenus.transform.Find("Controls");
        for (int i = 0; i < controlsList.childCount; i++)
        {
            Transform child = controlsList.GetChild(i);

            if (child.gameObject.CompareTag("KeybindSetting"))
            {
                string keyName = GetControl(child.gameObject.name).ToString();
                if (keyName.Contains("Alpha"))
                {
                    // Removes "Alpha" from main number keys
                    keyName = keyName.Replace("Alpha", "");
                }
                child.Find("Binding").GetComponent<TextMeshProUGUI>().text = keyName;
            }
            else if (child.gameObject.CompareTag("SliderSetting"))
            {
                child.Find("Slider").GetComponent<Slider>().value = GetSliderValue(child.gameObject.name);
            }
            
        }

        // Loads video settings
        Transform videoList = subMenus.transform.Find("Video");

        // Loads audio settings
        Transform audioList = subMenus.transform.Find("Audio");
        for (int i = 0; i < audioList.childCount; i++)
        {
            Transform child = audioList.GetChild(i);

            if (child.gameObject.CompareTag("SliderSetting"))
            {
                child.Find("Slider").GetComponent<Slider>().value = GetSliderValue(child.gameObject.name);
            }
            
        }
    }

    public void SaveSettings()
    {
        string settingsToSave = JsonUtility.ToJson(settings, true);
        File.WriteAllText("Settings.data", settingsToSave);
    }

    public void StartRebind(string controlName)
    {
        rebindPrompt.SetActive(true);
        currentRebind = controlName;
    }

    public KeyCode GetControl(string controlName)
    {
        switch (controlName)
        {
            case "moveForward":
                return settings.controls.moveForward;
            case "moveBackward":
                return settings.controls.moveBackward;
            case "moveLeft":
                return settings.controls.moveLeft;
            case "moveRight":
                return settings.controls.moveRight;
            default:
                return KeyCode.None;
        }
    }
    
    public void SetControl(string controlName, KeyCode newKey)
    {
        
        string keyName = newKey.ToString();

        switch (controlName)
        {
            case "moveForward":
                settings.controls.moveForward = newKey;
                //bindingText.GetComponent<TextMeshProUGUI>().text = keyName.Replace("Alpha","");
                break;
            case "moveBackward":
                settings.controls.moveBackward = newKey;
                break;
            case "moveLeft":
                settings.controls.moveLeft = newKey;
                break;
            case "moveRight":
                settings.controls.moveRight = newKey;
                break;
            
        }
    }

    public bool GetToggleValue(string optionName)
    {
        switch (optionName)
        {
            // Accessibility checkbox options
            case "isLeftHanded":
                return settings.accessibility.isLeftHanded;
            case "colourblindModeEnabled":
                return settings.accessibility.colourblindModeEnabled;
            case "dyslexiaSafeFontEnabled":
                return settings.accessibility.dyslexiaSafeFontEnabled;

            // Video checkbox options
            case "isFullscreen":
                return settings.video.isFullscreen;

            default:
                return false;
        }
    }

    public void SetToggleValue(GameObject option)
    {
        string optionName = option.name;
        GameObject checkbox = option.transform.Find("Checkbox").Find("Checkmark").gameObject;

        switch (optionName)
        {
            // Accessibility checkbox options
            case "isLeftHanded":
                settings.accessibility.isLeftHanded = !settings.accessibility.isLeftHanded;
                checkbox.SetActive(settings.accessibility.isLeftHanded);
                break;
            case "colourblindModeEnabled":
                settings.accessibility.colourblindModeEnabled = !settings.accessibility.colourblindModeEnabled;
                checkbox.SetActive(settings.accessibility.colourblindModeEnabled);
                break;
            case "dyslexiaSafeFontEnabled":
                settings.accessibility.dyslexiaSafeFontEnabled = !settings.accessibility.dyslexiaSafeFontEnabled;
                checkbox.SetActive(settings.accessibility.dyslexiaSafeFontEnabled);
                break;
            
            // Video checkbox options
            case "isFullscreen":
                settings.video.isFullscreen = !settings.video.isFullscreen;
                checkbox.SetActive(settings.video.isFullscreen);
                break;
            
        }
    }

    public float GetSliderValue(string settingName)
    {
        switch (settingName)
        {
            // Video slider settings
            case "camSensitivity":
                return settings.controls.camSensitivity;

            // Audio slider settings
            case "musicVolume":
                return settings.audio.musicVolume;
            case "sfxVolume":
                return settings.audio.sfxVolume;

            default:
                return 0.5f;
        }
    }

    public void SetSliderValue(GameObject setting)
    {
        string settingName = setting.name;
        GameObject slider = setting.transform.Find("Slider").gameObject;

        switch (settingName)
        {
            // Video slider settings
            case "camSensitivty":
                settings.controls.camSensitivity = slider.GetComponent<Slider>().value;
                break;

            // Audio slider settings
            case "musicVolume":
                settings.audio.musicVolume = slider.GetComponent<Slider>().value;
                break;
            case "sfxVolume":
                settings.audio.sfxVolume = slider.GetComponent<Slider>().value;
                break;
        }
    }

    public void ResetAllControls()
    {
        //settings.keybinds = Keybinds(false);
    }
}

[Serializable]
public class ClientSettings
{
    [Serializable] public struct AccessibilityOptions
    {
        public bool isLeftHanded;
        public bool colourblindModeEnabled;
        public bool dyslexiaSafeFontEnabled;

        public AccessibilityOptions(bool settingsLoaded)
        {
            isLeftHanded = false;
            colourblindModeEnabled = false;
            dyslexiaSafeFontEnabled = false;
        }
    }

    // Could be using the input manager for this, but for now manually setting the key codes should work fine
    [Serializable] public struct Controls
    {

        public KeyCode debugFlash;

        public float camSensitivity;
        public KeyCode moveForward;
        public KeyCode moveBackward;
        public KeyCode moveLeft;
        public KeyCode moveRight;

        public Controls(bool settingsLoaded)
        {
            // Will implement settings loaded check later
            debugFlash = KeyCode.Space;

            camSensitivity = 0.5f;
            moveForward = KeyCode.W;
            moveBackward = KeyCode.S;
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
        }
    }

    [Serializable] public struct Audio
    {
        public float musicVolume;
        public float sfxVolume;

        public Audio(bool settingsLoaded)
        {
            // Will implement settings loaded check later
            musicVolume = 0.5f;
            sfxVolume = 0.5f;
        }
    }

    [Serializable] public struct Video
    {
        public int width;
        public int height;
        public bool isFullscreen;

        public Video(bool settingsLoaded)
        {
            // Will implement settings loaded check later
            width = 1920;
            height = 1080;
            isFullscreen = true;
        }
    }

    public AccessibilityOptions accessibility;
    public Controls controls;
    public Audio audio;
    public Video video;

    public ClientSettings(bool settingsLoaded)
    {
        accessibility = new AccessibilityOptions(settingsLoaded);
        controls = new Controls(settingsLoaded);
        audio = new Audio(settingsLoaded);
        video = new Video(settingsLoaded);
    }
}