using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    public ClientSettings settings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Attempt to load settings from file, if not found then use default settings
        try
        {
            string settingsFile = File.ReadAllText("Settings.data");
            settings = new ClientSettings(true);
        }
        catch (FileNotFoundException)
        {
            print("Settings file not found, using default settings");
            settings = new ClientSettings(false);
        }
    }

    void Update()
    {
        // Might use for some debug things idk
    }

    public void ReadSettings()
    {
        GameObject tempText = settingsMenu.transform.Find("TempText").gameObject;
        tempText.GetComponent<TextMeshProUGUI>().text = JsonUtility.ToJson(settings);
    }

    public void SaveSettings()
    {
        string settingsToSave = JsonUtility.ToJson(settings, true);
        File.WriteAllText("Settings.data", settingsToSave);
    }
}

[Serializable]
public class ClientSettings
{
    // Could be using the input manager for this, but for now manually setting the key codes should work fine
    [Serializable] public struct Keybinds
    {
        public KeyCode moveForward;
        public KeyCode moveBackward;
        public KeyCode moveLeft;
        public KeyCode moveRight;

        public Keybinds(bool settingsLoaded)
        {
            // Will implement settings loaded check later
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