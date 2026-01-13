using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioListener))]
public class VoiceInputController : NetworkBehaviour
{
    // Base voice chat code sourced from https://github.com/Facepunch/Facepunch.Steamworks/issues/261#issuecomment-817334583

    [SerializeField] private float gain; 

    [SerializeField] private AudioSource source;

    [SerializeField] private Image voiceIcon;
    [SerializeField] private float iconDecayTime = 0.5f;

    private MemoryStream output;
    private MemoryStream stream;
    private MemoryStream input;

    private int optimalRate;
    private int clipBufferSize;
    private float[] clipBuffer;

    private int playbackBuffer;
    private int dataPosition;
    private int dataReceived;

    private bool canHearSelf = false;

    private List<float> recordedSamples = new List<float>();
    private List<float> storedSample = new List<float>();
    private bool isRecording = false;
    [SerializeField] private float silenceTimeout = 0.25f;

    private float lastVoiceTime;

    void Start()
    {
        optimalRate = (int)SteamUser.OptimalSampleRate; 

        clipBufferSize = optimalRate * 5;
        clipBuffer = new float[clipBufferSize];

        stream = new MemoryStream();
        output = new MemoryStream();
        input = new MemoryStream();

        source.clip = AudioClip.Create("VoiceData", optimalRate, 1, optimalRate, true, OnAudioRead, null);
        source.loop = true;
        source.Play();
    }

    public override void OnNetworkSpawn()
    {
        voiceIcon.enabled = false;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            canHearSelf = !canHearSelf;
        }

        SteamUser.VoiceRecord = Input.GetKey(KeyCode.V);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;

        isRecording = true;

        if (SteamUser.HasVoiceData) lastVoiceTime = Time.time;

        if (Time.time - lastVoiceTime < silenceTimeout)
        {
            voiceIcon.enabled = true;

            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

            VoiceServerRpc(stream.GetBuffer(), compressedWritten, NetworkManager.Singleton.LocalClientId);
        }

        if (isRecording && Time.time - lastVoiceTime > silenceTimeout)
        {
            if (recordedSamples.Count != 0)
            {
                storedSample = new List<float>(recordedSamples);
                recordedSamples.Clear();
            }

            voiceIcon.enabled = false;
        }
    }
       
    [ServerRpc(RequireOwnership = false)]
    public void VoiceServerRpc(byte[] compressed, int bytesWritten, ulong senderId)
    {
        VoiceDataClientRpc(compressed, bytesWritten, senderId);
    }

    [ClientRpc]
    public void VoiceDataClientRpc(byte[] compressed, int bytesWritten, ulong ownerId)
    {
        if (!canHearSelf && NetworkManager.Singleton.LocalClientId == ownerId) return;

        input.Write(compressed, 0, bytesWritten);
        input.Position = 0;

        int uncompressedWritten = SteamUser.DecompressVoice(input, bytesWritten, output);
        input.Position = 0;

        byte[] outputBuffer = output.GetBuffer();
        WriteToClip(outputBuffer, uncompressedWritten);
        output.Position = 0;
    }

    //[Client]
    private void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = 0;

            // Checks if there's audio data in the buffer that needs playing
            if (playbackBuffer > 0)
            {
                dataPosition = (dataPosition + 1) % clipBufferSize;

                data[i] = clipBuffer[dataPosition];

                playbackBuffer--;
            }
        }

    }

    //[Client]
    private void WriteToClip(byte[] uncompressed, int iSize)
    {
        for (int i = 0; i < iSize; i += 2)
        {
            // Insert converted float to buffer
            float converted = ((short)(uncompressed[i] | uncompressed[i + 1] << 8) / 32767.0f) * gain;
            clipBuffer[dataReceived] = converted;

            // Buffer loop
            dataReceived = (dataReceived +1) % clipBufferSize;

            playbackBuffer++;

            if(isRecording) recordedSamples.Add(converted);
        }
    }

    public AudioClip CreatePlayerVoiceClip()
    {
        if(storedSample.Count == 0) return null;

        AudioClip clip = AudioClip.Create(
            "PlayerVoice",
            storedSample.Count,
            1,
            optimalRate,
            false
        );

        clip.SetData(storedSample.ToArray(), 0);
        storedSample.Clear();

        return clip; 
    }
}