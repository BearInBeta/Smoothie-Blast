using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int ringCount; // Number of rings in the grid
    public GameObject hexPrefab; // Prefab for hex game object
    public float hexSize = 1f; // Size of each hex
    GameController gameController;
    private Dictionary<Vector2Int, Hex> hexes = new Dictionary<Vector2Int, Hex>(); // Dictionary to store hexes

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    // Method to generate the hex grid
    public void GenerateHexGrid()
    {
        hexes.Clear();
        // Create the rings
        for (int ring = -ringCount+1; ring < ringCount; ring++)
        {
            for (int i = -ringCount+1; i < ringCount; i++)
            {
                if(Mathf.Abs(ring + i) < ringCount)
                CreateHex(ring, i);
            }
        }
    }

    // Method to create a hex at given axial coordinates
    private void CreateHex(int q, int r)
    {
        Vector3 position = HexOffset(q, r);
        GameObject hexGO = Instantiate(hexPrefab, position, Quaternion.identity, transform);
        Hex hex = hexGO.GetComponent<Hex>();
        hex.setHexType(gameController.hexTypes[Random.Range(0,gameController.hexTypes.Length)]);
        hex.m_TextMeshPro.text = q + "," + r;
        hexes[new Vector2Int(q, r)] = hex;
    }
    //applies the current positions based on the key value pair
    private void applyPositions()
    {
        foreach (KeyValuePair<Vector2Int, Hex> hex in hexes)
        {
            hex.Value.gameObject.transform.position = HexOffset(hex.Key.x, hex.Key.y);
        }
    }
    // Method to calculate the offset of a hex at given axial coordinates
    private Vector3 HexOffset(int q, int r)
    {
        float x = hexSize * Mathf.Sqrt(3) * (q + r / 2f);
        float y = hexSize * 1.5f * r;
        return new Vector3(x, y, 0f);
    }


    //gets all hexes in the same ring
    public Dictionary<Vector2Int, Hex> GetHexesInRing(int ring)
    {
        Dictionary<Vector2Int, Hex> hexesInRing = new Dictionary<Vector2Int, Hex>();

        // Start from the initial hex and traverse the ring
        for (int q = -ring; q <= ring; q++)
        {
           for(int r = -ring; r <= ring; r++)
            {
                if ((Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(-q - r))/2 == ring)
                {
                    Vector2Int newHex = new Vector2Int(q, r);
                    hexesInRing.Add(newHex, hexes[newHex]);
                }
            }
        }

        return hexesInRing;
    }

    public void HighlightHexesInRing(int ring)
    {
        Dictionary<Vector2Int, Hex> hexesInRing = GetHexesInRing(ring);
        foreach (KeyValuePair <Vector2Int, Hex> hex in hexesInRing)
        {
            hex.Value.selectHex();
        }
    }

    public void RotateHexesInRing(int ring, bool clockwise)
    {
        Dictionary<Vector2Int, Hex> hexesInRing = GetHexesInRing(ring);

        foreach (KeyValuePair<Vector2Int, Hex> hex in hexesInRing)
        {
            if (clockwise)
            {
                hexes[MoveClockwise(hex.Key)] = hex.Value;
            }
            else
            {
                hexes[MoveCounterClockwise(hex.Key)] = hex.Value;
            }
        }
    }

    public Vector2Int MoveClockwise(Vector2Int currentHex)
    {
        int q = currentHex.x;
        int r = currentHex.y;
        int s = -q - r;

        return new Vector2Int(-s,-q);
    }

    public Vector2Int MoveCounterClockwise(Vector2Int currentHex)
    {
        int q = currentHex.x;
        int r = currentHex.y;
        int s = -q - r;

        return new Vector2Int(-r, -s);
    }

}
