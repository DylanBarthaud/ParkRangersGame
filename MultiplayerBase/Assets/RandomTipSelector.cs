using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomTipSelector : MonoBehaviour
{

    public TMP_Text TargetText;
    public List<String> toolTip = new List<String>();
 

    public void RandomizeTip()
    {
        String randomTip = toolTip[UnityEngine.Random.Range(0, toolTip.Count)];

        TargetText.text = randomTip;
    }


}
