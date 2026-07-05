using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;



public class LevelSelector : EditorWindow
{

    [MenuItem("Window/Scenes")]
    public static void ShowWindow()
    {
        GetWindow<LevelSelector>("Scenes");

    }

    void OnGUI()
    {
        GUILayout.Label("Scenes", EditorStyles.boldLabel);  

        if (GUILayout.Button("Find Scenes"))
        {
            string[] guids = AssetDatabase.FindAssets("t:SceneAsset");
            GenericMenu menu = new GenericMenu();
            menu.ShowAsContext();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                menu.AddItem(new GUIContent(asset.name), false, () => { OpenScene(path); });
                Debug.Log(asset.name);
            }
        }

        



    }

    private static void OpenScene(string path)
    {
        EditorSceneManager.OpenScene(path);
    }

}