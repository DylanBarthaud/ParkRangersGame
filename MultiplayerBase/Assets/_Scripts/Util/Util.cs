using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Utlility
{
    public static class ListExtentions
    {
        public static void Shuffle<T>(List<T> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                int roll = Random.Range(i, list.Count);
                var temp = list[i];
                list[i] = list[roll];
                list[roll] = temp;
            }
        }
    }
}