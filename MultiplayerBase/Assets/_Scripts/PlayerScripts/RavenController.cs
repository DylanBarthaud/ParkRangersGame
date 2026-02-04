using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RavenController : NetworkBehaviour
{
    [SerializeField] GameObject ravenObj;
    [SerializeField] PlayerInfoHolder playerInfoHolder;

    [Header("Raven Settings")]
    [SerializeField] int spawnInterval;
    [SerializeField, Range(0,100)] int spawnChance;
    [SerializeField] float spawnRadius;
    [SerializeField, Range(0,100)] int copyVoiceChance;

    [Header("Voice Collection Settings")]
    [SerializeField] int collectionInterval;
    [SerializeField] private List<AudioClip> crowAudioClips;
    [SerializeField] private int maximumAdditionalClips = 4;
    [SerializeField] private List<AudioClip> additionalClips; 

    private void Awake()
    {
        EventManager.instance.onTick += OnTick;
    }

    int localTick = 0;
    int collectionTick = 0; 
    private void OnTick(int tick)
    {
        localTick++;
        collectionTick++;

        if(collectionTick >= collectionInterval)
        {
            AudioClip newClip = GameManager.instance.GetAudioClip(OwnerClientId); 

            if(newClip != null)
            {
                if (additionalClips.Count < maximumAdditionalClips) additionalClips.Add(newClip);
                else shuffleClipList(newClip);
            }
        }

        if(localTick >= spawnInterval)
        {
            for (int i = 0; i < playerInfoHolder.GetPlayerInfo().ravenCount; i++)
            {
                int roll = Random.Range(0, 101);
                if (spawnChance >= roll)
                {
                    StartCoroutine(spawnRaven(Random.Range(0, (spawnInterval / 5) - 0.1f)));
                }
            }

            localTick = 0;
        }
    }

    private void shuffleClipList(AudioClip newClip)
    {
        int clipCount = additionalClips.Count;
        
        for(int i = clipCount - 1; i >= 0; i--) if (i != 0) additionalClips[i] = additionalClips[i - 1];

        additionalClips[0] = newClip;
    }

    private IEnumerator spawnRaven(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject crow = Instantiate(ravenObj, GetPointOnRing(), Quaternion.identity);
        Crow_AI crowScript = crow.GetComponent<Crow_AI>();

        int copyVoiceRoll = Random.Range(0, 101); 

        if(copyVoiceRoll <= copyVoiceChance && additionalClips.Count > 0)
        {
            int roll = Random.Range(0, additionalClips.Count);
            crowScript.SetNoiseData(additionalClips[roll]);
        }

        else
        {
            int roll = Random.Range(0, crowAudioClips.Count);
            crowScript.SetNoiseData(crowAudioClips[roll]);
        }
    }

    public Vector3 GetPointOnRing()
    {
        Vector3 point = new Vector3();

        float angle = Random.Range(0f, Mathf.PI * 2f);
        point = playerInfoHolder.GetPlayerInfo().position + new Vector3(
            Mathf.Cos(angle) * spawnRadius,
            0f,
            Mathf.Sin(angle) * spawnRadius
        );

        return point; 
    }
}
