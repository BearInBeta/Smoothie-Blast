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

    private void OnMouseEnter()
    {
        if (!Input.GetMouseButton(0))
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().chooseHexRing(this);
        }
    }
    public void selectHex()
    {
        fruitSelector.enabled = true;
    }

    public void deselectHex()
    {
        fruitSelector.enabled = false;
    }

}


