using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private NotebookHandler notebookHandler;
    private string questTextFilePath = Application.dataPath + "/Text/QuestText.txt";

    List<int> completedQuests = new List<int>();
    [SerializeField] List<SerializableKeysAndValues<string, int>> questKVs;
    Dictionary<string, int> questDictionary; 

    private void Awake()
    {
        foreach(var kv in questKVs) questDictionary.Add(kv.Key, kv.Value);
        notebookHandler.UpdatePageOneText(GetQuestText());
    }

    private string GetQuestText()
    {
        string text = string.Empty;
        string[] lines = File.ReadAllLines(questTextFilePath);

        List<string> uncompletedQuests = new List<string>();
        for(int i = 0; i < lines.Length; i++)
        {
            foreach (int completedQuestIndex in completedQuests)
            {
                if (i +1 == completedQuestIndex) break;
                uncompletedQuests.Add(lines[i]);
            }
        }

        foreach (string quest in uncompletedQuests)
        {
            text += quest + "\n";
        }

        return text;
    }
}

