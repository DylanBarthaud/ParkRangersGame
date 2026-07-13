using UnityEngine;
using UnityEngine.Audio;
public class GameMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer gameMixer;

    public void SetMasterVolume(float sliderValue)
    {
        gameMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetVoiceVolume(float sliderValue)
    {
        gameMixer.SetFloat("VoiceChatVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetRadioVolume(float sliderValue)
    {
        gameMixer.SetFloat("RadioChatVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        gameMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        gameMixer.SetFloat("SoundEffectVol", Mathf.Log10(sliderValue) * 20);
    }
    public void SetAmbienceVolume(float sliderValue)
    {
        gameMixer.SetFloat("AmbienceVol", Mathf.Log10(sliderValue) * 20);
    }
}
