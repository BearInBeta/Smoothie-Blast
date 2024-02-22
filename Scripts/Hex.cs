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
    public SpriteRenderer fruitHolder, fruitSelector;
    public bool toBeDestroyed;
    // Constructor
    private void Start()
    {
        

    }
    public void DestroyHex()
    {
        toBeDestroyed = true;
        GetComponent<SpriteRenderer>().enabled = false;
        fruitHolder.enabled = false;
        hexType = null;

    }
    public void setHexType(HexType hexType)
    {
        this.hexType = hexType;
        fruitHolder.sprite = this.hexType.Sprite;
        GetComponent<SpriteRenderer>().color = this.hexType.Color;
    }

    public void chooseRing()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().chooseHexRing(this);
    }
    public void selectHex(Color? color = null)
    {
        if(color == null)
        {
            fruitSelector.color = Color.white;
        }
        else
        {
            fruitSelector.color = (Color) color;
        }
        fruitSelector.enabled = true;
    }

    public void deselectHex()
    {
        fruitSelector.enabled = false;
    }

}


