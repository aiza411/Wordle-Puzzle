using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public Tile[] tiles { get; private set; }

    private void Awake()
    {
        tiles=GetComponentsInChildren<Tile>();
    }

    public string word
    {
        get
        {
            string word = "";
            for(int i = 0; i < tiles.Length; i++) 
            {
                word+=tiles[i].letter;
            }
            return word;
        }
    }
}
