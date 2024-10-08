using UnityEngine;
using UnityEngine.Tilemaps;

public enum BeaverAction
{
    None,
    Move,
    Chop,
    CollectResource,
    Repair,
    BuildDam,
    BuildLodge,
    BuildCanal
}

public class BeaverManager : MonoBehaviour
{
    public GameObject selectedBeaver = null;

    public static BeaverManager instance;

    public Tilemap waterTilemap;
    public Tilemap constructionTilemap;
    public Tilemap terrainTilemap;
    public Tilemap resourceTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap movementRangeTilemap;
    public Tile movementRangeTile;
    public Tile treeTile;
    public Tile rockTile;
    public Tile branchTile;
    public Tile saplingTile;
    public Tile damTile;
    public Tile lodgeTile;
    public Tile canalTile;

    public Texture2D cursorAxe;
    public Texture2D cursorRock;
    public Texture2D cursorDam;
    public Texture2D cursorRepair;
    public Texture2D cursorLodge;
    public Texture2D cursorCanal;
    private Texture2D currentCursor;

    public AstarPath astarPath;

    public int lodgesBuilt = 0;
    public int damsBuilt = 0;

    public BeaverAction currentAction = BeaverAction.None;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PerformClickAction();
            ResetCursor();
        }
        if (Input.GetMouseButtonDown(1) && currentAction != BeaverAction.None)
        {
            DeselectCurrentAction();
            UIManager.instance.HideBeaverUI();
        }
        if (Input.GetMouseButtonDown(1) && selectedBeaver != null)
        {
            selectedBeaver.transform.GetChild(0).gameObject.SetActive(false);
            selectedBeaver.transform.GetChild(1).gameObject.SetActive(false);
            selectedBeaver = null;
            movementRangeTilemap.ClearAllTiles();
            UIManager.instance.UpdateButtonVisibility(false);
            UIManager.instance.HideBeaverUI();
        }
    }
    public void DeselectCurrentAction()
    {
        currentAction = BeaverAction.None;
        ResetCursor();
        UIManager.instance.HideBeaverUI();
    }
    public void DeselectBeaver()
    {
        if (selectedBeaver != null)
        {
            selectedBeaver.transform.GetChild(0).gameObject.SetActive(false);
            selectedBeaver.transform.GetChild(1).gameObject.SetActive(false);
            selectedBeaver = null;
            movementRangeTilemap.ClearAllTiles();
            UIManager.instance.HideBeaverUI();
        }
    }

    private void PerformClickAction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.GetMask("Beaver");
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Beaver"))
            {
                SelectBeaver(hit.collider.gameObject);
                return;
            }
        }

        if (selectedBeaver != null)
        {
            Vector3Int tilePosition = movementRangeTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (currentAction != BeaverAction.None)
            {
                PerformSelectedAction(tilePosition);
            }
            else if (movementRangeTilemap.HasTile(tilePosition))
            {
                MoveBeaver(tilePosition);
            }
        }
    }

    void PerformSelectedAction(Vector3Int tilePosition)
    {
        Beaver beaverScript = selectedBeaver.GetComponent<Beaver>();
        if (beaverScript == null || beaverScript.hasActed)
        {
            UIManager.instance.DisplayMessage("Šis bebras jau atliko savo veiksmus");
            return;
        }

        switch (currentAction)
        {
            case BeaverAction.Chop:
                ChopAtTile(tilePosition);
                break;
            case BeaverAction.CollectResource:
                CollectResourceAtTile(tilePosition);
                break;
            case BeaverAction.Repair:
                RepairAtTile(tilePosition);
                break;
            case BeaverAction.BuildDam:
                BuildDamAtTile(tilePosition);
                break;
            case BeaverAction.BuildLodge:
                BuildLodgeAtTile(tilePosition);
                break;
            case BeaverAction.BuildCanal:
                BuildCanalAtTile(tilePosition);
                break;
        }

        currentAction = BeaverAction.None;
        movementRangeTilemap.ClearAllTiles();
        UIManager.instance.HideBeaverUI();
        selectedBeaver.transform.GetChild(0).gameObject.SetActive(false);
        selectedBeaver.transform.GetChild(1).gameObject.SetActive(false);
        selectedBeaver = null;
        UIManager.instance.UpdateButtonVisibility(false);
    }

    void ChopAtTile(Vector3Int tilePosition)
    {
        Vector3Int beaverPosition = terrainTilemap.WorldToCell(selectedBeaver.transform.position);
        Beaver beaverScript = selectedBeaver.GetComponent<Beaver>();
        Vector3 worldPosition = terrainTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);
        if (!IsAdjacentTile(beaverPosition, tilePosition))
        {
            UIManager.instance.DisplayMessage("Bebro nėra greta pasirinktos plytelės.");
            return;
        }

        if (resourceTilemap.GetTile(tilePosition) == treeTile)
        {
            resourceTilemap.SetTile(tilePosition, null);
            ResourceManager.instance.AddResource("wood", +3);
            ResourceManager.instance.AddResource("food", +1);
            UIManager.instance.ShowResourceChange(worldPosition, +3, "Mediena");
            UIManager.instance.ShowResourceChange(worldPosition, +1, "Maistas");
        }
        AudioManager.Instance.PlaySFX("ActionCutTree");
        beaverScript.hasActed = true;
    }

    void CollectResourceAtTile(Vector3Int tilePosition)
    {
        Vector3Int beaverPosition = terrainTilemap.WorldToCell(selectedBeaver.transform.position);
        Beaver beaverScript = selectedBeaver.GetComponent<Beaver>();
        if (!IsAdjacentTile(beaverPosition, tilePosition))
        {
            UIManager.instance.DisplayMessage("Pasirinkta plytelė nėra greta bebro.");
            return;
        }

        TileBase tile = resourceTilemap.GetTile(tilePosition);
        Vector3 worldPosition = terrainTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);
        if (tile == rockTile)
        {
            resourceTilemap.SetTile(tilePosition, null);
            UIManager.instance.DisplayMessage("Akmuo surinktas");
            ResourceManager.instance.AddResource("stone", +2);
            UIManager.instance.ShowResourceChange(worldPosition, +2, "Akmenys");
            GameManager.instance.UpdateWinConditionText();
        }
        else if (tile == branchTile)
        {
            resourceTilemap.SetTile(tilePosition, null);
            UIManager.instance.DisplayMessage("Šaka surinkta");
            ResourceManager.instance.AddResource("wood", +2);
            UIManager.instance.ShowResourceChange(worldPosition, +2, "Mediena");
            GameManager.instance.UpdateWinConditionText();
        }
        else if (tile == saplingTile)
        {
            resourceTilemap.SetTile(tilePosition, null);
            UIManager.instance.DisplayMessage("Sodinukas surinktas");
            ResourceManager.instance.AddResource("food", +4);
            UIManager.instance.ShowResourceChange(worldPosition, +4, "Maistas");
            GameManager.instance.UpdateWinConditionText();
        }
        else if (tile == TileManager.instance.fallenTreeTile)
        {
            resourceTilemap.SetTile(tilePosition, null);
            UIManager.instance.DisplayMessage("Nuvirtęs medis surinktas");
            ResourceManager.instance.AddResource("wood", +2);
            ResourceManager.instance.AddResource("food", +1);
            UIManager.instance.ShowResourceChange(worldPosition, +2, "Mediena");
            UIManager.instance.ShowResourceChange(worldPosition, +1, "Maistas");
            GameManager.instance.UpdateWinConditionText();
        }
        else
        {
            UIManager.instance.DisplayMessage("Pasirinktoje plytelė nėra surenkamų resursų.");
            return;
        }
        AudioManager.Instance.PlaySFX("ActionCollect");
        beaverScript.hasActed = true;
    }

    void RepairAtTile(Vector3Int tilePosition)
    {
        Beaver beaverScript = selectedBeaver.GetComponent<Beaver>();
        if (beaverScript == null || beaverScript.hasActed)
        {
            UIManager.instance.DisplayMessage("Šis bebras jau atliko savo veiksmus.");
            return;
        }

        TileBase currentTile = constructionTilemap.GetTile(tilePosition);
        Vector3 worldPosition = terrainTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);
        if (currentTile == RiverFlow.instance.damagedDamTile1 || currentTile == RiverFlow.instance.damagedDamTile2)
        {
            ResourceManager.instance.AddResource("wood", -2);
            UIManager.instance.ShowResourceChange(worldPosition, -2, "Mediena");
            GameManager.instance.UpdateWinConditionText();
            constructionTilemap.SetTile(tilePosition, RiverFlow.instance.undamagedDamTile);
            UIManager.instance.DisplayMessage("Užtvanka suremontuota");
            AudioManager.Instance.PlaySFX("ActionBuild");
            beaverScript.hasActed = true;
        }
        else if (currentTile == RiverFlow.instance.damagedLodgeTile1 || currentTile == RiverFlow.instance.damagedLodgeTile2 ||
                 currentTile == RiverFlow.instance.damagedLodgeTile3 || currentTile == RiverFlow.instance.damagedLodgeTile4)
        {
            ResourceManager.instance.AddResource("wood", -2);
            UIManager.instance.ShowResourceChange(worldPosition, -2, "Mediena");
            GameManager.instance.UpdateWinConditionText();
            constructionTilemap.SetTile(tilePosition, RiverFlow.instance.undamagedLodgeTile);
            UIManager.instance.DisplayMessage("Buveinė suremontuota");
            AudioManager.Instance.PlaySFX("ActionBuild");
            beaverScript.hasActed = true;
        }
        else
        {
            UIManager.instance.DisplayMessage("Pasirinktoje plytelė nėra remontuojamos užtvankos ar plytelės");
        }
    }

    void BuildDamAtTile(Vector3Int tilePosition)
    {
        Vector3Int beaverPosition = terrainTilemap.WorldToCell(selectedBeaver.transform.position);
        Vector3 worldPosition = terrainTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);
        Beaver beaverScript = selectedBeaver.GetComponent<Beaver>();
        if (!IsAdjacentTile(beaverPosition, tilePosition))
        {
            UIManager.instance.DisplayMessage("Bebro nėra greta pasirinktos plytelės.");
            return;
        }
        if (waterTilemap.HasTile(tilePosition) || terrainTilemap.HasTile(tilePosition) && !obstacleTilemap.HasTile(tilePosition) && !constructionTilemap.HasTile(tilePosition) && !resourceTilemap.HasTile(tilePosition))
        {
            ResourceManager.instance.AddResource("wood", -3);
            ResourceManager.instance.AddResource("stone", -1);
            UIManager.instance.ShowResourceChange(worldPosition, -3, "Mediena");
            UIManager.instance.ShowResourceChange(worldPosition, -1, "Akmenys");
            constructionTilemap.SetTile(tilePosition, damTile);
            UpdateGraph(tilePosition);
            AudioManager.Instance.PlaySFX("ActionBuild");
            damsBuilt++;
            GameManager.instance.UpdateWinConditionText();
            beaverScript.hasActed = true;
        }
    }
    void BuildLodgeAtTile(Vector3Int tilePosition)
    {
        Vector3Int beaverPosition = terrainTilemap.WorldToCell(selectedBeaver.transform.position);
        Vector3 worldPosition = terrainTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);
        Beaver beaverScript = selectedBeaver.GetComponent<Beaver>();
        if (!IsAdjacentTile(beaverPosition, tilePosition))
        {
            UIManager.instance.DisplayMessage("Bebro nėra greta pasirinktos plytelės.");
            return;
        }
        if (waterTilemap.HasTile(tilePosition) && !obstacleTilemap.HasTile(tilePosition) && !constructionTilemap.HasTile(tilePosition) && !resourceTilemap.HasTile(tilePosition))
        {
            ResourceManager.instance.AddResource("wood", -5);
            ResourceManager.instance.AddResource("stone", -3);
            UIManager.instance.ShowResourceChange(worldPosition, -5, "Mediena");
            UIManager.instance.ShowResourceChange(worldPosition, -3, "Akmenys");
            constructionTilemap.SetTile(tilePosition, lodgeTile);
            lodgesBuilt++;

            GameManager.instance.UpdateWinConditionText();
            UIManager.instance.DisplayMessage("Buveine pastatyta, iš viso buveinių: " + lodgesBuilt);
            UpdateGraph(tilePosition);
            AudioManager.Instance.PlaySFX("ActionBuild");
            beaverScript.hasActed = true;
        }
        else
        {
            UIManager.instance.DisplayMessage("Pasirinktoje plytelė negalima statyti buveinės: " + tilePosition);
        }
    }

    void BuildCanalAtTile(Vector3Int tilePosition)
    {
        Beaver beaverScript = selectedBeaver.GetComponent<Beaver>();
        if (IsOnOrAdjacentToWater(tilePosition) && !obstacleTilemap.HasTile(tilePosition) && !constructionTilemap.HasTile(tilePosition))
        {
            constructionTilemap.SetTile(tilePosition, canalTile);
            UIManager.instance.DisplayMessage("Pasirinktoje plytelė negalima statyti kanalo: " + tilePosition);
            beaverScript.hasActed = true;
        }
        else
        {
            UIManager.instance.DisplayMessage("Pasirinktoje plytelė negalima statyti buveinės: " + tilePosition);
        }
    }

    public void UpdateGraph(Vector3Int tilePosition)
    {
        Vector3 worldPosition = waterTilemap.CellToWorld(tilePosition);
        Bounds bounds = new Bounds(worldPosition, new Vector3(1, 1, 0));

        astarPath.UpdateGraphs(bounds);
    }
    ////////////
    /////////////
    //Set action
    /////////////
    ////////////
    public void SetActionToChop()
    {
        currentAction = BeaverAction.Chop;
        SetCustomCursor(cursorAxe);
        UIManager.instance.DisplayMessage("Pasiriktas veiksmas kirsti medį.");
    }

    public void SetActionToCollectResource()
    {
        currentAction = BeaverAction.CollectResource;
        SetCustomCursor(cursorRock);
        UIManager.instance.DisplayMessage("Pasiriktas veiksmas surinkti resursą.");
    }
    public void SetActionToRepair()
    {
        if (ResourceManager.instance.wood >= 2)
        {
            currentAction = BeaverAction.Repair;
            SetCustomCursor(cursorRepair);
            UIManager.instance.DisplayMessage("Pasiriktas veiksmas pataisyti užtvanką arba buveinę");
        }
        else
        {
            UIManager.instance.DisplayMessage("Nepakanka resursų. Reikalinga: 2 medienos");
        }
    }

    public void SetActionToBuildDam()
    {
        if (ResourceManager.instance.wood >= 3 && ResourceManager.instance.stone >= 1)
        {
            currentAction = BeaverAction.BuildDam;
            SetCustomCursor(cursorDam);
            UIManager.instance.DisplayMessage("Pasiriktas veiksmas statyti užtvanką");
        }
        else
        {
            UIManager.instance.DisplayMessage("Nepakanka resursų. Reikalinga: 3 medienos, 1 akmens.");
        }
    }

    public void SetActionToBuildLodge()
    {
        if (ResourceManager.instance.wood >= 5 && ResourceManager.instance.stone >= 2)
        {
            currentAction = BeaverAction.BuildLodge;
            SetCustomCursor(cursorLodge);
            UIManager.instance.DisplayMessage("Pasiriktas veiksmas statyti buveinę");
        }
        else
        {
            UIManager.instance.DisplayMessage("Nepakanka resursų. Reikalinga: 5 medienos ir 3 akmenų");
        }
    }

    public void SetActionToBuildCanal()
    {
        currentAction = BeaverAction.BuildCanal;
        SetCustomCursor(cursorCanal);
        UIManager.instance.DisplayMessage("Pasiriktas veiksmas statyti kanalą");
    }

    void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void SetCustomCursor(Texture2D cursorType)
    {
        Vector2 hotspot = new Vector2(cursorType.width / 2, cursorType.height / 2);
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }
    
    ////////
    ////////
    //MOVEMENT SELECT
    ////////
    void ShowMovementRange(Vector3Int beaverPosition)
    {
        movementRangeTilemap.ClearAllTiles();

        int range = 1; 
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int tilePosition = new Vector3Int(beaverPosition.x + x, beaverPosition.y + y, beaverPosition.z);

                if (!obstacleTilemap.HasTile(tilePosition) && (x != 0 || y != 0))
                {
                    movementRangeTilemap.SetTile(tilePosition, movementRangeTile);
                }
            }
        }
    }

    void MoveBeaver(Vector3Int tilePosition)
    {
        if (selectedBeaver == null) return;

        Vector3 worldPosition = movementRangeTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);

        if (IsBeaverAtPosition(worldPosition))
        {
            UIManager.instance.DisplayMessage("Čia jau yra kitas bebras.");
            return;
        }

        Beaver beaver = selectedBeaver.GetComponent<Beaver>();
        if (beaver == null) return;

        float movementCost = CalculateMovementCost(tilePosition);

        if (!beaver.UseMovementPoints(movementCost))
        {
            UIManager.instance.DisplayMessage("Nepakanka ėjimo taškų.");
            return;
        }

        selectedBeaver.transform.position = worldPosition;

        movementRangeTilemap.ClearAllTiles();
        if (IsOnOrAdjacentToWater(tilePosition))
        {
            ShowMovementRange(tilePosition);
        }
        else
        {
            selectedBeaver.transform.GetChild(0).gameObject.SetActive(false);
            selectedBeaver.transform.GetChild(1).gameObject.SetActive(false);
            selectedBeaver = null;
            UIManager.instance.UpdateButtonVisibility(false);
            UIManager.instance.HideBeaverUI();
        }
    }
    bool IsBeaverAtPosition(Vector3 position)
    {
        Collider2D collider = Physics2D.OverlapBox(position, new Vector2(0.1f, 0.1f), 0);
        if (collider != null && collider.CompareTag("Beaver"))
        {
            return true;
        }
        return false;
    }

    float CalculateMovementCost(Vector3Int tilePosition)
    {
        if (waterTilemap.HasTile(tilePosition)) return 1f;
        if (terrainTilemap.HasTile(tilePosition)) return 4f;                                              
        return 0f;
    }
    bool IsAdjacentTile(Vector3Int beaverPosition, Vector3Int tilePosition)
    {
        return Mathf.Abs(beaverPosition.x - tilePosition.x) <= 1 && Mathf.Abs(beaverPosition.y - tilePosition.y) <= 1;
    }
    bool IsOnOrAdjacentToWater(Vector3Int tilePosition)
    {
        for (int x = -1; x <= 2; x++)
        {
            for (int y = -1; y <= 2; y++)
            {
                Vector3Int neighborPosition = new Vector3Int(tilePosition.x + x, tilePosition.y + y, tilePosition.z);
                if (waterTilemap.HasTile(neighborPosition))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void SelectBeaver(GameObject beaver)
    {
        if (!RiverFlow.instance.hasCollidedWithRiverEnd)
        {
            UIManager.instance.DisplayMessage("Palaukite kol vyksta upės simuliacija");
            return;
        }

        Beaver beaverScript = beaver.GetComponent<Beaver>();
        if (beaverScript == null)
        {
            Debug.Log("Nera bebro");
            return;
        }
        if (selectedBeaver != null)
        {
            selectedBeaver.transform.GetChild(0).gameObject.SetActive(false);
            selectedBeaver.transform.GetChild(1).gameObject.SetActive(false);
            selectedBeaver = null;
            movementRangeTilemap.ClearAllTiles();
            UIManager.instance.UpdateButtonVisibility(false);
            UIManager.instance.HideBeaverUI();
        }

        selectedBeaver = beaver;
        SpriteRenderer spriteRenderer = selectedBeaver.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector3Int gridPosition = TileManager.instance.waterTilemap.WorldToCell(selectedBeaver.transform.position);
            if (TileManager.instance.waterTilemap.HasTile(gridPosition))
            {
                selectedBeaver.transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (TileManager.instance.terrainTilemap.HasTile(gridPosition))
            {
                selectedBeaver.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        ShowMovementRange(terrainTilemap.WorldToCell(selectedBeaver.transform.position));
        UIManager.instance.UpdateButtonVisibility(true);
        UIManager.instance.ShowBeaverUI();
        UIManager.instance.UpdateBeaverStats(beaverScript);
    }
}
