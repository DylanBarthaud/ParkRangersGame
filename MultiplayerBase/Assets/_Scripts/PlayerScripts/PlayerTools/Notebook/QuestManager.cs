using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private NotebookHandler notebookHandler;
    private string questTextFilePath = Application.streamingAssetsPath + "/Text/QuestText.txt";

    List<int> completedQuests = new List<int>();
    [SerializeField] List<SerializableKeysAndValues<string, int>> questKVs;
    Dictionary<string, int> questDictionary = new Dictionary<string, int>(); 

    private void Awake()
    {
        foreach(var kv in questKVs) questDictionary.Add(kv.Key, kv.Value);
        notebookHandler.UpdatePageOneText(GetQuestText());

        EventManager.instance.onQuestComplete += OnQuestComplete;
    }

    private string GetQuestText()
    {
        string text = string.Empty;
        string[] lines = File.ReadAllLines(questTextFilePath);

        List<string> uncompletedQuests = new List<string>();
        for(int i = 0; i < lines.Length; i++)
        {
            bool isComplete = false; 
            foreach (int completedQuestIndex in completedQuests)
            {
                if (i +1 == completedQuestIndex)
                {
                    isComplete = true;
                    break;
                }
            }
            if(!isComplete) uncompletedQuests.Add(lines[i]);
        }

        foreach (string quest in uncompletedQuests) text += quest + "\n";


        return text;
    }

    private void OnQuestComplete(string quest)
    {
        int completedQuestIndex = questDictionary[quest]; 
        completedQuests.Add(completedQuestIndex);
        notebookHandler.UpdatePageOneText(GetQuestText());
    }
}

