using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.ParticleSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HexGrid : MonoBehaviour
{
    private int ringCount; // Number of rings in the grid
    public GameObject hexPrefab; // Prefab for hex game object
    public float hexSize = 1f; // Size of each hex
    GameController gameController;
    public float speed;
    private Dictionary<Vector2Int, Hex> hexes = new Dictionary<Vector2Int, Hex>(); // Dictionary to store hexes
    public bool isRotating = false;
    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    /*Hex generation methods*/
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
    // Method to create a hex at given axial coordinates
    private void CreateHex(int q, int r)
    {
        bool createsCluster = true;
        HexType randHex = null;
        while (createsCluster)
        {
            randHex = gameController.hexTypes[Random.Range(0, gameController.hexTypes.Length)];
            List<Vector2Int> cluster = new List<Vector2Int>();
            findCluster(new Vector2Int(q, r), cluster, randHex);
            createsCluster = cluster.Count >= 3;

        }


        Vector3 position = HexOffset(q, r);
        GameObject hexGO = Instantiate(hexPrefab, position, Quaternion.identity, transform);
        Hex hex = hexGO.GetComponent<Hex>();
        hex.setHexType(randHex);
        hex.m_TextMeshPro.text = q + "," + r;
        hexes[new Vector2Int(q, r)] = hex;
    }

    // Method to calculate the offset of a hex at given axial coordinates
    private Vector3 HexOffset(int q, int r)
    {
        float x = hexSize * Mathf.Sqrt(3) * (q + r / 2f);
        float y = hexSize * 1.5f * r;
        return new Vector3(x, y, (float)-getRing(q, r) / 100f);
    }

    /*End hex generation methods*/

    /*finding hexes, clusters, and rings */
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
    //gets all hexes in the same ring
    public Dictionary<Vector2Int, Hex> GetHexesInRing(int ring)
    {
        Dictionary<Vector2Int, Hex> hexesInRing = new Dictionary<Vector2Int, Hex>();

        // Start from the initial hex and traverse the ring
        for (int q = -ring; q <= ring; q++)
        {
            for (int r = -ring; r <= ring; r++)
            {
                if (getRing(q, r) == ring)
                {
                    Vector2Int newHex = new Vector2Int(q, r);
                    if (hexes.ContainsKey(newHex))
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
    public void findCluster(Vector2Int first, List<Vector2Int> cluster, HexType hexType)
    {
        cluster.Add(first);
        List<Vector2Int> neighbours = getNeighbours(first);
        foreach (Vector2Int neighbor in neighbours)
        {
            if (!cluster.Contains(neighbor) && hexes[neighbor].hexType != null && hexes[neighbor].hexType.Equals(hexType))
            {
                findCluster(neighbor, cluster, hexType);
            }
        }
    }
    private List<Vector2Int> checkForClusters()
    {
        List<Vector2Int> checkedHexes = new List<Vector2Int>();
        List<Vector2Int> toDestroy = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Hex> hex in hexes)
        {
            if (!checkedHexes.Contains(hex.Key))
            {
                List<Vector2Int> cluster = new List<Vector2Int>();
                findCluster(hex.Key, cluster, hex.Value.hexType);
                foreach (Vector2Int neighbor in cluster)
                {
                    checkedHexes.Add(neighbor);
                    if (cluster.Count >= 3)
                    {
                        toDestroy.Add(neighbor);
                    }
                }

            }
        }

        return toDestroy;

    }
    public List<Vector2Int> getNeighbours(Vector2Int first, bool south = false)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();


        Vector2Int[] allNeighbours = { new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1) };
        Vector2Int[] southNeighbours = { new Vector2Int(1, -1), new Vector2Int(0, -1) };
        Vector2Int[] possibleNeighbours = allNeighbours;
        if (south)
        {
            possibleNeighbours = southNeighbours;
        }
        foreach (Vector2Int possibleNeighbour in possibleNeighbours)
        {
            if (hexes.ContainsKey(first + possibleNeighbour))
            {
                neighbours.Add(first + possibleNeighbour);
            }
        }

        return neighbours;

    }
    /*End finding hexes, clusters, and rings */

    /* Moving hexes */
    //applies the current positions based on the key value pair

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
    private IEnumerator rotationWaiter(GameObject hex, Vector2Int to)
    {
        isRotating = true;
        Vector3 currentPos = hex.transform.position;
        Vector3 targetPos = HexOffset(to.x, to.y);
        float distance = Vector3.Distance(currentPos, targetPos);

        while (distance > 0.01f)
        {
            currentPos = hex.transform.position;
            distance = Vector3.Distance(currentPos, targetPos);
            yield return null;
        }
        List<Vector2Int> toDestroy = checkForClusters();
        foreach(Vector2Int vector2Int in toDestroy)
        {
            hexes[vector2Int].selectHex(Color.yellow);
        }
        isRotating = false;
        
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
            // Get the index of the current element

            if (hexesInRing.Last().Equals(hex))
            {
                StartCoroutine(rotationWaiter(hex.Value.gameObject, newKey));
            }
            hexes[newKey] = hex.Value;


        }

    }

    public Vector2Int MoveClockwise(Vector2Int currentHex)
    {
        int q = currentHex.x;
        int r = currentHex.y;
        int s = -q - r;

        return new Vector2Int(-s, -q);
    }

    public Vector2Int MoveCounterClockwise(Vector2Int currentHex)
    {
        int q = currentHex.x;
        int r = currentHex.y;
        int s = -q - r;

        return new Vector2Int(-r, -s);
    }

    /*End moving hexes*/

    /*Highlighting hexes*/
    public void HighlightHexesInRing(int ring)
    {
        dehighlightAll();
        Dictionary<Vector2Int, Hex> hexesInRing = GetHexesInRing(ring);
        foreach (KeyValuePair <Vector2Int, Hex> hex in hexesInRing)
        {
            hex.Value.selectHex();
        }
    }
    public void dehighlightAll()
    {
        foreach (KeyValuePair<Vector2Int, Hex> hex in hexes)
        {
            hex.Value.deselectHex();
        }
    }
    /*End Highlighting hexes*/

}
