using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    HexGrid hg;
    public int ringCount;
    public HexType[] hexTypes;
    public TMP_Dropdown dropdown;
    private int selectedRing = 0;
    // Start is called before the first frame update
    void Start()
    {
        hg = GetComponent<HexGrid>();
        hg.GenerateHexGrid(ringCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SelectRing()
    {
        ChooseRing(dropdown.value);
    }
    public void ChooseRing(int ring)
    {
        if (ring == 0) return;
        selectedRing = ring;
        hg.HighlightHexesInRing(selectedRing);
    }
    public void chooseHexRing(Hex hex)
    {
        ChooseRing(hg.GetHexRing(hex));
    }
    public void rotateRing(bool clockwise)
    {
        if (hg.isRotating || selectedRing == 0) return;
        hg.RotateHexesInRing(selectedRing, clockwise);
        selectedRing = 0;
        hg.dehighlightAll();
    }
    // Function to remove all options from a TMP Dropdown and set options from 0 to a specified number
    /*private void SetDropdownOptions()
    {
        // Clear existing options
        dropdown.ClearOptions();

        // Create a list to hold the options
        TMP_Dropdown.OptionDataList options = new TMP_Dropdown.OptionDataList();

        // Add options from 0 to the specified number
        for (int i = 0; i < ringCount; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }

        // Assign the options to the dropdown
    }*/
}
