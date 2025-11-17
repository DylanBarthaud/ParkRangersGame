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

    private Coroutine coroutine = null; // Initiates the coroutine to null

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
        source.clip = AudioClip.Create("VoiceData", 256, 1, optimalRate, true, OnAudioRead, null);
        source.loop = true;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        SteamUser.VoiceRecord = Input.GetKey(KeyCode.V); // Push to talk using the V key -- Might see if this can be changed into a changeable keybind 

        if (SteamUser.HasVoiceData)
        {
            if (coroutine != null) // If the HideIcon() coroutine is active, end it
            {
                Debug.Log("Coroutine ended");
                StopCoroutine(coroutine);
                coroutine = null;
            }
            voiceIcon.enabled = true;

            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

            CmdVoice(stream.GetBuffer(), compressedWritten);
        }
        else
        {
            if (coroutine == null) // Only starts the HideIcon() coroutine if it isn't already active
            {
                Debug.Log("Coroutine started");
                coroutine = StartCoroutine(HideIcon());
            }
        }

    }

    // Icon hiding coroutine - Hides the voice icon after a set amount of seconds to prevent flickering
    private IEnumerator HideIcon()
    {
        yield return new WaitForSeconds(iconDecayTime);
        voiceIcon.enabled = false;
    }

    //[Command()]
    public void CmdVoice(byte[] compressed, int bytesWritten)
    {
        /*var exclude = new List<ulong> { OwnerClientId };
        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(id => !exclude.Contains(id)).ToArray()
            }
        };
        */
        VoiceDataClientRpc(compressed, bytesWritten, OwnerClientId);
    }



    [ClientRpc]
    public void VoiceDataClientRpc(byte[] compressed, int bytesWritten, ulong ownerId)
    {
       // if (ownerId == OwnerClientId) return;

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
            float converted = (short)(uncompressed[i] | uncompressed[i + 1] << 8) / 32767.0f;
            clipBuffer[dataReceived] = converted;

            // Buffer loop
            dataReceived = (dataReceived +1) % clipBufferSize;

            playbackBuffer++;
        }
    }
}
