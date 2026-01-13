using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VoiceInputController))]
public class RavenController : MonoBehaviour
{
    [SerializeField] GameObject ravenObj;
    [SerializeField] PlayerInfoHolder playerInfoHolder;

    private VoiceInputController voiceInputController;

    [Header("Raven Settings")]
    [SerializeField] int spawnInterval;
    [SerializeField, Range(0,100)] int spawnChance;
    [SerializeField] float spawnRadius;

    [Header("Voice Collection Settings")]
    [SerializeField] int collectionInterval;
    [SerializeField] private List<AudioClip> crowAudioClips;

    private void Awake()
    {
        voiceInputController = gameObject.GetComponent<VoiceInputController>();

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
            AudioClip newClip = voiceInputController.CreatePlayerVoiceClip();
            if (newClip != null) crowAudioClips.Add(newClip);
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

    public IEnumerator spawnRaven(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Spawn");

        GameObject crow = Instantiate(ravenObj, GetPointOnRing(), Quaternion.identity);
        Crow_AI crowScript = crow.GetComponent<Crow_AI>();

        int roll = Random.Range(0, crowAudioClips.Count);
        crowScript.SetNoiseData(crowAudioClips[roll]); 
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
