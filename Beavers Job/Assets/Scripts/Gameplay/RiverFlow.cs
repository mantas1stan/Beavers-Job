using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using Pathfinding;

public class RiverFlow : MonoBehaviour
{
    public static RiverFlow instance;

    public Tilemap waterTilemap;
    public Tilemap obstacleTilemap;
    public Tile waterTile;

    public Tilemap constructionTilemap;
    public Tile damagedDamTile1;
    public Tile damagedDamTile2;
    public Tile undamagedDamTile;

    public Tile undamagedLodgeTile;
    public Tile damagedLodgeTile1;
    public Tile damagedLodgeTile2;
    public Tile damagedLodgeTile3;
    public Tile damagedLodgeTile4;

    public bool hasCollidedWithRiverEnd = false;
    private bool canDamage = true;

    public float deltaX = 0.15f;
    public float deltaY = 0.15f;

    public TileManager TileManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(MoveObject());
    }
    IEnumerator MoveObject()
    {
        while (true)
        {
            transform.position = new Vector2(transform.position.x + deltaX, transform.position.y);
            yield return new WaitForSeconds(0.3f);

            transform.position = new Vector2(transform.position.x - deltaX, transform.position.y);
            yield return new WaitForSeconds(0.3f);

            transform.position = new Vector2(transform.position.x - deltaX, transform.position.y);
            yield return new WaitForSeconds(0.3f);

            transform.position = new Vector2(transform.position.x + deltaX, transform.position.y);
            yield return new WaitForSeconds(0.5f);

            transform.position = new Vector2(transform.position.x, transform.position.y - deltaY);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OverlayTerrainTiles(transform.position);
        if (other.gameObject.CompareTag("Water"))
        {
            Vector3 hitPosition = other.ClosestPoint(transform.position);
            Vector3Int cellPosition = waterTilemap.WorldToCell(hitPosition);

            TileManager.instance.ResetWaterTileAgeAndReplace(cellPosition);
        }
        RemoveResourceTiles(transform.position);
        RemoveTerrainTiles(transform.position);
        if (canDamage)
        {
            DamageDamTile(transform.position);
            StartCoroutine(DamageCooldown());
        }
    }

    void OverlayTerrainTiles(Vector3 worldPosition)
    {
        Vector3Int cellPosition = waterTilemap.WorldToCell(worldPosition);

        if (!waterTilemap.HasTile(cellPosition) && !obstacleTilemap.HasTile(cellPosition))
        {
            waterTilemap.SetTile(cellPosition, waterTile);
            TileManager.instance.AddAffectedWaterTile(cellPosition);
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(0.25f);
        canDamage = true;
    }
    IEnumerator Scan()
    {
        yield return new WaitForSeconds(0.3f);
        AstarPath.active.Scan();
    }
    void DamageDamTile(Vector3 worldPosition)
    {
        Vector3Int cellPosition = constructionTilemap.WorldToCell(worldPosition);
        TileBase currentTile = constructionTilemap.GetTile(cellPosition);

        if (currentTile == undamagedDamTile)
        {
            constructionTilemap.SetTile(cellPosition, damagedDamTile1);
        }
        else if (currentTile == damagedDamTile1) 
        {
            constructionTilemap.SetTile(cellPosition, damagedDamTile2);
        }
        else if (currentTile == damagedDamTile2)
        {
            constructionTilemap.SetTile(cellPosition, null);
            StartCoroutine(Scan());
        }

        if (currentTile == undamagedLodgeTile)
        {
            constructionTilemap.SetTile(cellPosition, damagedLodgeTile1);
        }
        else if (currentTile == damagedLodgeTile1)
        {
            constructionTilemap.SetTile(cellPosition, damagedLodgeTile2);
        }
        else if (currentTile == damagedLodgeTile2)
        {
            constructionTilemap.SetTile(cellPosition, damagedLodgeTile3);
        }
        else if (currentTile == damagedLodgeTile3)
        {
            constructionTilemap.SetTile(cellPosition, damagedLodgeTile4);
        }
        else if (currentTile == damagedLodgeTile4)
        {
            constructionTilemap.SetTile(cellPosition, null);
            StartCoroutine(Scan());
        }
    }
    void RemoveResourceTiles(Vector3 worldPosition)
    {
        Vector3Int cellPosition = TileManager.resourceTilemap.WorldToCell(worldPosition);
        TileBase currentTile = TileManager.resourceTilemap.GetTile(cellPosition);

        if (currentTile == TileManager.stoneTile || currentTile == TileManager.branchTile || currentTile == TileManager.saplingTile)
        {
            TileManager.resourceTilemap.SetTile(cellPosition, null);
        }
    }
    void RemoveTerrainTiles(Vector3 worldPosition)
    {
        Vector3Int cellPosition = TileManager.terrainTilemap.WorldToCell(worldPosition);
        TileBase currentTile = TileManager.terrainTilemap.GetTile(cellPosition);

        if (currentTile == TileManager.grassRuleTile)
        {
            TileManager.terrainTilemap.SetTile(cellPosition, null);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RiverEnd"))
        {
            hasCollidedWithRiverEnd = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RiverEnd"))
        {
            hasCollidedWithRiverEnd = false;
        }
    }

    public bool IsCollidingWithRiverEnd()
    {
        return hasCollidedWithRiverEnd;
    }
}