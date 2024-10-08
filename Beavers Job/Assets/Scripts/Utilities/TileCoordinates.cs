using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class TileCoordinates : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap tilemap;
    public TextMeshProUGUI coordinatesText;

    void Update()
    {
        DisplayCoordinates();
    }

    void DisplayCoordinates()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        coordinatesText.text = $"Coordinates: {cellPosition.x}, {cellPosition.y}";
    }
}