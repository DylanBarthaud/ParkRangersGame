using Steamworks;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class VoiceInputController : MonoBehaviour
{
    // Code sourced from https://github.com/Facepunch/Facepunch.Steamworks/issues/261#issuecomment-817334583

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

        source =GetComponent<AudioSource>();
        source.clip = AudioClip.Create("VoiceData", 256, 1, optimalRate, true, OnAudioRead, null);
        source.loop = true;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        SteamUser.VoiceRecord = Input.GetKey(KeyCode.V);

        if (SteamUser.HasVoiceData)
        {
            int compressedWritten = SteamUser.ReadVoiceData(stream);
            stream.Position = 0;

            CmdVoice(stream.GetBuffer(), compressedWritten);
        }

    }

    //[Command()]
    public void CmdVoice(byte[] compressed, int bytesWritten)
    {
        RpcVoiceData(compressed, bytesWritten);
    }


    //[ClientRpc(excludeOwner = true)]
    public void RpcVoiceData(byte[] compressed, int bytesWritten)
    {
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
