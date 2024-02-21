using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hex : MonoBehaviour
{
    // Properties
    public HexType hexType;
    public TMPro.TextMeshPro m_TextMeshPro;
    public SpriteRenderer fruitHolder;
    // Constructor
    private void Start()
    {
        

    }

    public void setHexType(HexType hexType)
    {
        this.hexType = hexType;
        fruitHolder.sprite = this.hexType.Sprite;
        GetComponent<SpriteRenderer>().color = this.hexType.Color;
    }

    public void selectHex()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    

}


