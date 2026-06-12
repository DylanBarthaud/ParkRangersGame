using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class InspectController : MonoBehaviour
{
    [SerializeField] private GameObject objectNameBG;
    [SerializeField] private TextMeshProUGUI objectNameUI;

    [SerializeField] private float onScreentimer;
    [SerializeField] private Text extraInfoUI;
    [SerializeField] private GameObject extraInfoBG;
    [HideInInspector] public bool startTimer;
    private float timer;

    private void Start()
    {
        objectNameBG.SetActive(false);
        extraInfoBG.SetActive(false);
    }

    private void Update()
    {
        if (startTimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                ClearAdditionalInfo();
                startTimer = false;
            }
        }
    }

    public void ShowName(string objectName, Color textColour)
    {
        objectNameBG.SetActive(true);
        objectNameUI.text = objectName;
        objectNameUI.color = textColour;
        Debug.Log("ShowNameActivated");
    }

    public void HideName()
    {
        objectNameBG.SetActive(false);
        objectNameUI.text = "";
        Debug.Log("HideNameActivated");
    }

    public void ShowAdditionalInfo(string newInfo)
    {
        timer = onScreentimer;
        startTimer = true;
        extraInfoBG.SetActive(false);
        extraInfoUI.text = newInfo;
    }

    void ClearAdditionalInfo()
    {
        extraInfoBG.SetActive(false) ;
        extraInfoUI.text = "";
    }

}