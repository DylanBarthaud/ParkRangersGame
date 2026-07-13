using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SettingsDataManager : MonoBehaviour
{

    private AudioMixer gameMixer;
    public float masterVol;
    public float voiceVol;
    public float radioVol;
    public float musicVol;
    public float sfxVol;
    public float ambienceVol;

    public void SaveSettings()
    {
        SettingsData settingsData = new SettingsData();
        //settingsData.masterVol = gameMixer.GetFloat("MasterVol", );


    }

    public void LoadSettings() 
    { 

    }
}
