using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
//using Pathfinding;

public class DamBuilder : MonoBehaviour
{
    public static DamBuilder instance;

    public bool isBuildingMode = false;
    public Tilemap waterTilemap;
    public Tilemap constructionTilemap;
    public Tilemap terrainTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap resourceTilemap;

    public Tile damTile;
    public Tile waterTile;
    public Tile lodgeTile;
    public Tile canalTile;

    public AstarPath astarPath;

    public enum BuildingType
    {
        Dam,
        Lodge,
        Canal
    }

    public BuildingType currentBuildingType = BuildingType.Dam;

    public void SelectDam()
    {
        currentBuildingType = BuildingType.Dam;
        isBuildingMode = true;
    }

    public void SelectLodge()
    {
        currentBuildingType = BuildingType.Lodge;
        isBuildingMode = true;
    }

    public void SelectCanal()
    {
        currentBuildingType = BuildingType.Canal;
        isBuildingMode = true;
    }

    void Update()
    {
        if (isBuildingMode && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            PlaceBuildingAtMousePosition();
        }
    }

    public void ToggleBuildingMode()
    {
        isBuildingMode = !isBuildingMode;
        Debug.Log("Building Mode: " + isBuildingMode);
    }

    void PlaceBuildingAtMousePosition()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;
        Vector3Int tilePosition = terrainTilemap.WorldToCell(worldPoint);

        bool canPlaceBuilding = !obstacleTilemap.HasTile(tilePosition) && !constructionTilemap.HasTile(tilePosition) && !resourceTilemap.HasTile(tilePosition);

        if (canPlaceBuilding)
        {
            Tile tileToPlace = null;

            switch (currentBuildingType)
            {
                case BuildingType.Dam:
                    tileToPlace = damTile;
                    break;
                case BuildingType.Lodge:
                    tileToPlace = lodgeTile;
                    break;
                case BuildingType.Canal:
                    tileToPlace = canalTile;
                    break;
            }

            if (tileToPlace != null)
            {
                constructionTilemap.SetTile(tilePosition, tileToPlace);
                UpdateGraph(tilePosition);
            }
        }
        else
        {
            Debug.Log($"Cannot place {currentBuildingType} here due to obstacles or existing constructions.");
        }
    }

    public void UpdateGraph(Vector3Int tilePosition)
    {
        Vector3 worldPosition = waterTilemap.CellToWorld(tilePosition);
        Bounds bounds = new Bounds(worldPosition, new Vector3(1, 1, 0));

        astarPath.UpdateGraphs(bounds);
    }
}
