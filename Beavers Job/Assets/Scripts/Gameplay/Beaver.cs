using UnityEngine;

public class Beaver : MonoBehaviour
{
    public static Beaver instance;

    public string beaverName;
    public Sprite beaverImage;
    public Sprite waterTileSprite;
    public Sprite terrainTileSprite;
    public bool hasActed = false;
    public float movementPoints = 4f;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer on the beaver GameObject");
        }
    }

    private void Update()
    {
        UpdateSpriteBasedOnTile();
    }

    private void UpdateSpriteBasedOnTile()
    {
        Vector3Int gridPosition = GetCurrentTilePosition();
        if (TileManager.instance.waterTilemap.HasTile(gridPosition))
        {
            spriteRenderer.sprite = waterTileSprite;
        }
        else if (TileManager.instance.terrainTilemap.HasTile(gridPosition))
        {
            spriteRenderer.sprite = terrainTileSprite;
        }
    }

    private Vector3Int GetCurrentTilePosition()
    {
        return TileManager.instance.waterTilemap.WorldToCell(transform.position);
    }

    public void ResetActions()
    {
        hasActed = false;
        UIManager.instance.UpdateBeaverStats(this);
    }
    public void ResetBeaver()
    {
        hasActed = false;
        movementPoints = 4f;
        UIManager.instance.UpdateBeaverStats(this);
    }
    public void EndTurnBeaver()
    {
        hasActed = true;
        movementPoints = 0f;
        UIManager.instance.UpdateBeaverStats(this);
    }

    public void ResetMovementPoints()
    {
        movementPoints = 4f;
        UIManager.instance.UpdateBeaverStats(this);
    }

    public bool UseMovementPoints(float cost)
    {
        if (movementPoints >= cost)
        {
            movementPoints -= cost;
            UIManager.instance.UpdateBeaverStats(this);
            return true;
        }
        return false;
    }

}