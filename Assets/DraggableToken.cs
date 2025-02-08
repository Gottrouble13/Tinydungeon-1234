using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableToken : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private bool isDragging = false;
    public string tokenType; // "PlayerToken", "Enemy1", "Enemy2"

    void Start()
    {
        cam = Camera.main;

        if (string.IsNullOrEmpty(tokenType))
        {
            tokenType = gameObject.name.Replace("Token", ""); // Auto-detect type
            Debug.Log($"Auto-Setting Token Type: {tokenType}");
        }
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPos() + offset;
            newPosition.z = -1f; // Ensure token stays in front
            transform.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        SnapToGrid();
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10f;
        return cam.ScreenToWorldPoint(mousePoint);
    }

    void SnapToGrid()
    {
        Vector3Int cellPosition = GridManager.instance.tilemap.WorldToCell(transform.position);

        Debug.Log($"SnapToGrid called for {gameObject.name} at {cellPosition}");

        if (gameObject.name == "PlayerToken")
        {
            if (GridManager.instance.spawnPositions.ContainsKey("PlayerToken") &&
                GridManager.instance.CanPlacePlayer())
            {
                Debug.Log($"PlayerToken placed at {cellPosition}");
                transform.position = GridManager.instance.tilemap.GetCellCenterWorld(
                    GridManager.instance.spawnPositions["PlayerToken"]
                );
                GridManager.instance.PlacePlayer(cellPosition, gameObject);
            }
            else
            {
                Debug.LogError($"Invalid player placement!");
            }
        }
        else if (tokenType.StartsWith("Enemy")) // Generic check for all enemies
        {
            Debug.Log($"Moving {tokenType} to {cellPosition}");
            transform.position = GridManager.instance.tilemap.GetCellCenterWorld(cellPosition);

            Debug.Log($"Calling RespawnToken() for {tokenType}");
            GridManager.instance.RespawnToken(gameObject);
        }
    }

}
