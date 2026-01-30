using TMPro;
using UnityEngine;

public class NotebookHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pageOneText;
    [SerializeField] TextMeshProUGUI pageTwoText;

    public void UpdatePageOneText(string text)
    {
        UpdateText(text, pageOneText); 
    }

    public void UpdatePageTwoText(string text)
    {
        UpdateText(text, pageTwoText);
    }

    private void UpdateText(string text, TextMeshProUGUI page)
    {
        page.text = text;
    }
}
