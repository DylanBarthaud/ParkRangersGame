using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    private GameObject rebindPrompt;
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
            if (Input.GetKey(settings.keybinds.debugFlash))
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

    public void ReadSettings()
    {
        GameObject tempText = settingsMenu.transform.Find("TempText").gameObject;
        tempText.GetComponent<TextMeshProUGUI>().text = JsonUtility.ToJson(settings);

        Transform controlsList = settingsMenu.transform.Find("Controls");
        for (int i = 0; i < controlsList.childCount; i++) // Will eventually move all control rebinds into their own canvas for simplicity, but this should work fine for now
        {
            Transform child = controlsList.GetChild(i);
            string keyName = GetControl(child.gameObject.name).ToString();
            if (keyName.Contains("Alpha"))
            {
                // Removes "Alpha" from main number keys
                keyName = keyName.Replace("Alpha", "");
            }
            child.Find("Binding").GetComponent<TextMeshProUGUI>().text = keyName;
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
                return settings.keybinds.moveForward;
            case "moveBackward":
                return settings.keybinds.moveBackward;
            case "moveLeft":
                return settings.keybinds.moveLeft;
            case "moveRight":
                return settings.keybinds.moveRight;
            default:
                return KeyCode.None;
        }
    }
    
    public void SetControl(string controlName, KeyCode newKey)
    {
        switch (controlName)
        {
            case "moveForward":
                settings.keybinds.moveForward = newKey;
                break;
            case "moveBackward":
                settings.keybinds.moveBackward = newKey;
                break;
            case "moveLeft":
                settings.keybinds.moveLeft = newKey;
                break;
            case "moveRight":
                settings.keybinds.moveRight = newKey;
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
    // Could be using the input manager for this, but for now manually setting the key codes should work fine
    [Serializable] public struct Keybinds
    {
        public KeyCode debugFlash;

        public KeyCode moveForward;
        public KeyCode moveBackward;
        public KeyCode moveLeft;
        public KeyCode moveRight;

        public Keybinds(bool settingsLoaded)
        {
            // Will implement settings loaded check later
            debugFlash = KeyCode.Space;

            moveForward = KeyCode.W;
            moveBackward = KeyCode.S;
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
        }
    }

    [Serializable] public struct Audio
    {
        public float volume;

        public Audio(bool settingsLoaded)
        {
            // Will implement settings loaded check later
            volume = 0.5f;
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

    public Keybinds keybinds;
    public Audio audioSettings;
    public Video videoSettings;

    public ClientSettings(bool settingsLoaded)
    {
        keybinds = new Keybinds(settingsLoaded);
        audioSettings = new Audio(settingsLoaded);
        videoSettings = new Video(settingsLoaded);
    }
}