using Steamworks;
using System.Collections;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioListener))]
public class VoiceInputController : NetworkBehaviour
{
    // Code sourced from https://github.com/Facepunch/Facepunch.Steamworks/issues/261#issuecomment-817334583

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
    private Coroutine coroutine = null;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            canHearSelf = !canHearSelf;
        }

        SteamUser.VoiceRecord = Input.GetKey(KeyCode.V);

        if (SteamUser.HasVoiceData)
        {
            if (coroutine != null) 
            {
                Debug.Log("Coroutine ended");
                StopCoroutine(coroutine);
                coroutine = null;
            }
            voiceIcon.enabled = true;

            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

            VoiceServerRpc(stream.GetBuffer(), compressedWritten, NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            if (coroutine == null) // Only starts the HideIcon() coroutine if it isn't already active
            {
                coroutine = StartCoroutine(HideIcon());
            }
        }

    }
       
    [ServerRpc(RequireOwnership = false)]
    public void VoiceServerRpc(byte[] compressed, int bytesWritten, ulong senderId)
    {
        VoiceDataClientRpc(compressed, bytesWritten, senderId);
    }

    private IEnumerator HideIcon()
    {
        yield return new WaitForSeconds(iconDecayTime);
        voiceIcon.enabled = false;
    }

    [ClientRpc]
    public void VoiceDataClientRpc(byte[] compressed, int bytesWritten, ulong ownerId)
    {
        if (!canHearSelf && NetworkManager.Singleton.LocalClientId == ownerId) return;
        else
        {
            if (coroutine == null)
            {
                Debug.Log("Coroutine started");
                coroutine = StartCoroutine(HideIcon());
            }
        }

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