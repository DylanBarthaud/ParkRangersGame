using Steamworks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class VoiceInputController : NetworkBehaviour
{
    // Code sourced from https://github.com/Facepunch/Facepunch.Steamworks/issues/261#issuecomment-817334583

    [SerializeField] private float gain; 

    private AudioSource source;

    private MemoryStream output;
    private MemoryStream stream;
    private MemoryStream input;

    private int optimalRate;
    private int clipBufferSize;
    private float[] clipBuffer;

    private int playbackBuffer;
    private int dataPosition;
    private int dataReceived;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        optimalRate = (int)SteamUser.OptimalSampleRate; // Sets the sample rate to Steam's native sample rate

        clipBufferSize = optimalRate * 5;
        clipBuffer = new float[clipBufferSize];

        stream = new MemoryStream();
        output = new MemoryStream();
        input = new MemoryStream();

        source = GetComponent<AudioSource>();
        source.clip = AudioClip.Create("VoiceData", optimalRate, 1, optimalRate, true, OnAudioRead, null);
        source.loop = true;
        source.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return; 
 
        //SteamUser.VoiceRecord = Input.GetKey(KeyCode.V);
        SteamUser.VoiceRecord = true;

        if (SteamUser.HasVoiceData)
        {
            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

            Debug.Log("Steam User has voice");
            VoiceServerRpc(stream.GetBuffer(), compressedWritten, NetworkManager.Singleton.LocalClientId);
        }

    }
       
    [ServerRpc(RequireOwnership = false)]
    public void VoiceServerRpc(byte[] compressed, int bytesWritten, ulong senderId)
    {
        Debug.Log("VoiceServerRpc");
        VoiceDataClientRpc(compressed, bytesWritten, senderId);
    }

    [ClientRpc]
    public void VoiceDataClientRpc(byte[] compressed, int bytesWritten, ulong senderId)
    {
        //if (NetworkManager.Singleton.LocalClientId == senderId) return;

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
        }
    }
}