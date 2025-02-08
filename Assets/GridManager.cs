using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [SerializeField] public Tilemap tilemap;  // Reference to the game board's Tilemap
    public Dictionary<string, Vector3Int> spawnPositions = new Dictionary<string, Vector3Int>();
    public Dictionary<string, GameObject> tokenPrefabs = new Dictionary<string, GameObject>(); // Store token prefabs
    private Dictionary<string, GameObject> activeTokens = new Dictionary<string, GameObject>(); // Track placed tokens

    public int maxPlayers = 8; // Max number of player tokens allowed
    private int currentPlayers = 0;

    void Awake()
    {
        if (instance == null) instance = this;  // Ensure there is only one GridManager
    }

    void Start()
    {
        DefineSpawnPositions();
    }

    void DefineSpawnPositions()
    {
        // Define player spawn positions
        spawnPositions["PlayerToken"] = new Vector3Int(-11, 3, 0); // Adjust as needed


        spawnPositions["Enemy1Token"] = new Vector3Int(-4, 3, 0); // Adjust
        spawnPositions["Enemy2Token"] = new Vector3Int(-4,2, 0); // Adjust

        Debug.Log("Spawn positions defined!");

        // Store prefab references (drag prefabs in Unity Inspector)
        tokenPrefabs["PlayerToken"] = Resources.Load<GameObject>("Prefabs/PlayerToken");
        tokenPrefabs["Enemy1Token"] = Resources.Load<GameObject>("Prefabs/Enemy1");
        tokenPrefabs["Enemy2Token"] = Resources.Load<GameObject>("Prefabs/Enemy2");
    }

    public bool CanPlacePlayer()
    {
        return currentPlayers < maxPlayers;
    }

    public void PlacePlayer(Vector3Int cellPosition, GameObject token)
    {
        Debug.Log($"PlacePlayer() called for {token.name} at {cellPosition}");

        if (CanPlacePlayer())
        {
            currentPlayers++;
            Debug.Log($"Player placed at {cellPosition}. Current players: {currentPlayers}/{maxPlayers}");

            // Ensure token stays at the placed position
            token.transform.position = GridManager.instance.tilemap.GetCellCenterWorld(cellPosition);

            // Call RespawnToken() but DO NOT destroy old token
            Debug.Log($"Respawning a new player token...");
            RespawnToken(token);
        }
        else
        {
            Debug.LogError($"Cannot place player: Max players reached!");
        }
    }



    public void RespawnToken(GameObject oldToken)
    {
        string tokenName = oldToken.name.Replace("(Clone)", "").Trim();

        Debug.Log($"RespawnToken called for: {tokenName}");

        Debug.Log("Available spawn positions:");
        foreach (var key in spawnPositions.Keys)
        {
            Debug.Log($" {key} -> {spawnPositions[key]}");
        }

        if (spawnPositions.ContainsKey(tokenName))
        {
            Vector3Int gridPosition = spawnPositions[tokenName];
            Vector3 spawnPos = tilemap.CellToWorld(gridPosition);

            // Apply offset for alignment
            spawnPos.x += 0.5f;
            spawnPos.y += 0.5f;

            Debug.Log($"Spawning new {tokenName} at {spawnPos}");

            if (tokenPrefabs.ContainsKey(tokenName))
            {
                // Create a new enemy instead of replacing the old one
                GameObject newToken = Instantiate(tokenPrefabs[tokenName], spawnPos, Quaternion.identity);
                newToken.name = tokenName;
            }
            else
            {
                Debug.LogError($"Prefab not found for {tokenName}!");
            }
        }
        else
        {
            Debug.LogError($"No spawn position found for {tokenName}!");
        }
    }




}
