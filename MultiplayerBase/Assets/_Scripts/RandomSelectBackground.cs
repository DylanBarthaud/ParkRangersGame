using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSelectBackground : MonoBehaviour
{

    public Image TargetImage; 
    public List<Sprite> bgImage = new List<Sprite>();

    public void RandomizeImage()
    {
        Sprite randomBG = bgImage[UnityEngine.Random.Range(0, bgImage.Count)];

        TargetImage.sprite = randomBG;
    }

    
}
