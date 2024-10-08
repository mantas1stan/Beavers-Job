using UnityEngine;
using UnityEngine.Tilemaps;
//NEREIKIA SITO SCRIPTO GALIMA VISKA TRINTI 99%
public class DamTileManager : MonoBehaviour
{
    public Tilemap constructionTilemap;
    public Tile normalDamTile;
    public Tile damagedDamTile1;
    public Tile damagedDamTile2;
    public Tile normalLodgeTile;
    public Tile damagedLodgeTile1;
    public Tile damagedLodgeTile2;
    public Tile damagedLodgeTile3;
    public Tile damagedLodgeTile4;
    public Tile waterTile;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Colission found");
        if (collision.CompareTag("Dam"))
        {
            Vector3Int cellPosition = constructionTilemap.WorldToCell(collision.transform.position);
            TileBase currentTile = constructionTilemap.GetTile(cellPosition);
            Debug.Log("Tag found");

            if (constructionTilemap.HasTile(cellPosition))
            {
                constructionTilemap.SetTile(cellPosition, waterTile);
                constructionTilemap.SetTile(cellPosition, damagedDamTile1);
            }
            if (currentTile == normalDamTile)
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
            }
            
            if (currentTile == normalLodgeTile)
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
            }
        }
    }
}
