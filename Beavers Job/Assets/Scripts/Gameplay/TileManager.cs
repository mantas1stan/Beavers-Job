using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[System.Serializable]
public class TileData
{
    public Vector3Int position;
    public int age;
    public TileState state;

    public TileData(Vector3Int pos, TileState initialState)
    {
        position = pos;
        state = initialState;
        age = 0;
    }
}

[System.Serializable]
public class TreeData
{
    public Vector3Int position;
    public int age;
    public bool isFullyGrown;

    public TreeData(Vector3Int pos)
    {
        position = pos;
        age = 0;
        isFullyGrown = false;
    }
}

public enum TileState { Water, StillWater, Vegetation, ShallowWater, ToBeRemoved }

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    public Tilemap terrainTilemap;
    public Tilemap waterTilemap;
    public Tilemap resourceTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap constructionTilemap;

    public Tile baseWaterTile;
    public Tile vegetationTile;
    public Tile shallowWaterTile;
    public Tile stillWaterTile;
    public Tile damTile;
    public Tile saplingTile;
    public Tile treeTile;
    public Tile fallenTreeTile;
    public Tile branchTile;
    public Tile stoneTile;
    public Tile boulderTile;
    public RuleTile grassRuleTile;

    public List<TileData> affectedTiles = new List<TileData>();
    public List<TreeData> trees = new List<TreeData>();

    private void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            SpawnRandomSapling();
        }
        for (int i = 0; i < 30; i++)
        {
            Vector3Int randomPosition = GetRandomPosition();
            SpawnTree(randomPosition);
        }
        for (int i = 0; i < 15; i++)
        {
            Vector3Int randomPosition = GetRandomPosition();
            SpawnBranch(randomPosition);
        }
        for (int i = 0; i < 12; i++)
        {
            SpawnStone();
        }

    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void PlaceDam(Vector3Int gridPosition)
    {
        if (waterTilemap.GetTile(gridPosition) != null)
        {
            waterTilemap.SetTile(gridPosition, null);
            constructionTilemap.SetTile(gridPosition, damTile);
        }
    }
    public void UpdateAffectedTiles()
    {
        List<TileData> tilesToRemove = new List<TileData>();

        foreach (TileData tile in affectedTiles)
        {
            tile.age++;
            if (tile.age == 2 && tile.state == TileState.Water)
            {
                tile.state = TileState.StillWater;
                waterTilemap.SetTile(tile.position, stillWaterTile);
            }
            else if (tile.age == 4 && tile.state == TileState.StillWater)
            {
                tile.state = TileState.Vegetation;
                waterTilemap.SetTile(tile.position, vegetationTile);
            }
            else if (tile.age == 6 && tile.state == TileState.Vegetation)
            {
                tile.state = TileState.ShallowWater;
                waterTilemap.SetTile(tile.position, shallowWaterTile);
            }
            else if (tile.age == 8 && tile.state == TileState.ShallowWater)
            {
                tile.state = TileState.ToBeRemoved;
                waterTilemap.SetTile(tile.position, null);
                terrainTilemap.SetTile(tile.position, grassRuleTile);
                tilesToRemove.Add(tile);
            }
        }

        foreach (TileData tile in tilesToRemove)
        {
            affectedTiles.Remove(tile);
        }
    }
    public void AddAffectedWaterTile(Vector3Int position)
    {
        TileData newTile = new TileData(position, TileState.Water);
        affectedTiles.Add(newTile);
        waterTilemap.SetTile(position, baseWaterTile);
    }
    public void ResetWaterTileAgeAndReplace(Vector3Int position)
    {
        TileData existingTileData = affectedTiles.Find(tileData => tileData.position == position);

        if (existingTileData != null)
        {
            existingTileData.age = 0;
            existingTileData.state = TileState.Water;
        }
        else
        {
            TileData newTile = new TileData(position, TileState.Water);
            affectedTiles.Add(newTile);
        }
        waterTilemap.SetTile(position, baseWaterTile);
    }

    public void UpdateTrees()
    {
        List<TreeData> treesToRemove = new List<TreeData>();
        foreach (TreeData tree in trees)
        {
            if (waterTilemap.GetTile(tree.position) != null)
            {
                if (!tree.isFullyGrown)
                {
                    treesToRemove.Add(tree);
                    resourceTilemap.SetTile(tree.position, null);
                }
                else
                {
                    if (tree.age < 6)
                    {
                        if (tree.age == 4)
                        {
                            resourceTilemap.SetTile(tree.position, fallenTreeTile);
                        }
                        tree.age++;
                    }
                    else
                    {
                        treesToRemove.Add(tree);
                        resourceTilemap.SetTile(tree.position, null);
                    }
                }
            }
            else if (tree.age < 4)
            {
                tree.age++;
                if (tree.age == 4)
                {
                    resourceTilemap.SetTile(tree.position, treeTile);
                    tree.isFullyGrown = true;
                }
            }
        }

        foreach (TreeData tree in treesToRemove)
        {
            trees.Remove(tree);
        }
    }

    public void SpawnRandomSapling()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3Int randomPosition = GetRandomPosition();

            bool isTileEmpty = terrainTilemap.GetTile(randomPosition) != null
                                && waterTilemap.GetTile(randomPosition) == null
                                && constructionTilemap.GetTile(randomPosition) == null
                                && obstacleTilemap.GetTile(randomPosition) == null
                                && resourceTilemap.GetTile(randomPosition) == null;

            if (isTileEmpty)
            {
                TreeData newSapling = new TreeData(randomPosition);
                trees.Add(newSapling);
                resourceTilemap.SetTile(randomPosition, saplingTile);
                return;
            }
        }

        Debug.Log("Failed to place a sapling. No empty tile found.");
    }

    public void SpawnTree(Vector3Int position)
    {
        if (terrainTilemap.GetTile(position) != null &&
            waterTilemap.GetTile(position) == null &&
            constructionTilemap.GetTile(position) == null &&
            obstacleTilemap.GetTile(position) == null)
        {
            TreeData newTree = new TreeData(position)
            {
                isFullyGrown = true,
                age = 4
            };
            trees.Add(newTree);
            resourceTilemap.SetTile(position, treeTile);
        }
    }

    public void SpawnBranch(Vector3Int position)
    {
        if (terrainTilemap.GetTile(position) != null &&
            waterTilemap.GetTile(position) == null &&
            constructionTilemap.GetTile(position) == null &&
            obstacleTilemap.GetTile(position) == null)
        {
            resourceTilemap.SetTile(position, branchTile);
        }
    }
    public void SpawnStone()
    {
        Vector3Int position = GetRandomPosition();
        if (terrainTilemap.GetTile(position) != null &&
            waterTilemap.GetTile(position) == null &&
            constructionTilemap.GetTile(position) == null &&
            obstacleTilemap.GetTile(position) == null)
        {
            resourceTilemap.SetTile(position, stoneTile);
        }
    }
    public void SpawnBoulder(Vector3Int position)
    {
        if (terrainTilemap.GetTile(position) != null &&
            waterTilemap.GetTile(position) == null &&
            constructionTilemap.GetTile(position) == null &&
            resourceTilemap.GetTile(position) == null &&
            obstacleTilemap.GetTile(position) == null)
        {
            obstacleTilemap.SetTile(position, boulderTile);
        }
    }
    private Vector3Int GetRandomPosition()
    {
        int x = Random.Range(-10, 9);
        int y = Random.Range(-7, 6);
        return new Vector3Int(x, y, 0);
    }
}
