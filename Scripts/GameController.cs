using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    HexGrid hg;
    public HexType[] hexTypes;
    // Start is called before the first frame update
    void Start()
    {
        hg = GetComponent<HexGrid>();
        hg.GenerateHexGrid();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
