using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    private int ringCount; // Number of rings in the grid
    public GameObject hexPrefab; // Prefab for hex game object
    public float hexSize = 1f; // Size of each hex
    GameController gameController;
    public float speed;
    private Dictionary<Vector2Int, Hex> hexes = new Dictionary<Vector2Int, Hex>(); // Dictionary to store hexes

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    // Method to generate the hex grid
    public void GenerateHexGrid(int ringCount)
    {
        this.ringCount = ringCount;
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
    public int GetHexRing(Hex hex)
    {
        Vector2Int coordinates = FindHexCoordinates(hex);
        return getRing(coordinates.x, coordinates.y);
    }
    public Vector2Int FindHexCoordinates(Hex hexToFind)
    {
        foreach (KeyValuePair<Vector2Int, Hex> hex in hexes)
        {
            if(hexToFind.gameObject == hex.Value.gameObject)
            {
                return hex.Key;
            }
        }
        return Vector2Int.zero;
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

    private IEnumerator movePositions(GameObject hex, Vector2Int to)
    {
        Vector3 currentPos = hex.transform.position;
        Vector3 targetPos = HexOffset(to.x, to.y);
        float distance = Vector3.Distance(currentPos, targetPos);

        while (distance > 0.01f)
        {
            currentPos = Vector3.MoveTowards(currentPos, targetPos, speed * getRing(to.x, to.y) * Time.deltaTime);
            hex.transform.position = currentPos;
            distance = Vector3.Distance(currentPos, targetPos);
            yield return null;
        }

        hex.transform.position = targetPos;

    }
    // Method to calculate the offset of a hex at given axial coordinates
    private Vector3 HexOffset(int q, int r)
    {
        float x = hexSize * Mathf.Sqrt(3) * (q + r / 2f);
        float y = hexSize * 1.5f * r;
        return new Vector3(x, y, (float)-getRing(q, r) / 100f);
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
                if (getRing(q, r) == ring)
                {
                    Vector2Int newHex = new Vector2Int(q, r);
                    hexesInRing.Add(newHex, hexes[newHex]);
                }
            }
        }

        return hexesInRing;
    }

    private int getRing(int q, int r)
    {
        return (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(-q - r)) / 2;
    }

    public void HighlightHexesInRing(int ring)
    {
        foreach (KeyValuePair<Vector2Int, Hex> hex in hexes)
        {
            hex.Value.deselectHex();
        }
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
            Vector2Int newKey;
            if (clockwise)
            {
                 newKey = MoveClockwise(hex.Key);
            }
            else
            {
                newKey = MoveCounterClockwise(hex.Key);
            }
            StartCoroutine(movePositions(hex.Value.gameObject, newKey));
            hexes[newKey] = hex.Value;
            

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
