using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int turnCount = 0;
    public TextMeshProUGUI turnDisplayText;

    public GameObject riverStartObject;
    public Transform riverStartPoint;

    public int lodgesBuilt = 0;
    public int winFoodRequirement = 100;
    public int winLodgeRequirement = 3;
    public int loseTurnLimit = 20;
    public TextMeshProUGUI winConditionText;

    public LevelConfig currentLevelConfig;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        turnDisplayText.gameObject.SetActive(false);
        riverStartObject.SetActive(false);
    }
    private void Start()
    {
        UpdateWinConditionText();
        AudioManager.Instance.PlayMusic("Ambient");
        EndTurn();
    }

    public void EndTurn()
    {
        AudioManager.Instance.PlaySFX("WaterSwoosh");
        turnCount++;
        BeaverManager.instance.DeselectCurrentAction();
        BeaverManager.instance.DeselectBeaver();
        Beaver[] allBeavers = FindObjectsOfType<Beaver>();
        foreach (Beaver beaver in allBeavers)
        {
            beaver.EndTurnBeaver();
        }
        UIManager.instance.UpdateButtonVisibility(false);
        UIManager.instance.DisplayMessage("Ėjimas " + (turnCount - 1) + " baigtas.");
        DeductFoodForBeavers();
        UIManager.instance.UpdateResourceUI(ResourceManager.instance.wood, ResourceManager.instance.food, ResourceManager.instance.stone);
        UIManager.instance.ShowResourceChange(Vector3.zero, -allBeavers.Length, "Bebrų suvartotas maistas");

        CheckWinCondition();
        CheckLoseCondition();

        TileManager.instance.UpdateAffectedTiles();
        TileManager.instance.UpdateTrees();
        TileManager.instance.SpawnRandomSapling();
        if (turnCount % 2 == 0)
        {
            TileManager.instance.SpawnStone();
        }

        StartCoroutine(ShowTurnDisplayBriefly());
        StartCoroutine(ActivateAndMoveRiverStartAfterDelay());

        UpdateWinConditionText();
    }
    public void UpdateWinConditionText()
    {
        if (winConditionText != null)
        {
            winConditionText.text = "Laimėjimo sąlygos\n";
            if (currentLevelConfig.requiredFood > 0)
                winConditionText.text += $"Maisto surinkti: {ResourceManager.instance.food}/{currentLevelConfig.requiredFood}\n";
            if (currentLevelConfig.requiredLodges > 0)
                winConditionText.text += $"Buveinių pastatyti: {BeaverManager.instance.lodgesBuilt}/{currentLevelConfig.requiredLodges}\n";
            if (currentLevelConfig.requiredDams > 0)
                winConditionText.text += $"Užtvankų pastatyti: {BeaverManager.instance.damsBuilt}/{currentLevelConfig.requiredDams}\n";
            if (currentLevelConfig.requiredWood > 0)
                winConditionText.text += $"Medienos surinkti: {ResourceManager.instance.wood}/{currentLevelConfig.requiredWood}\n";
            if (currentLevelConfig.maxTurns > 0)
                winConditionText.text += $"Liko ėjimų: {currentLevelConfig.maxTurns - turnCount}";
        }
    }
    public void OnResourceOrLodgeUpdated()
    {
        UpdateWinConditionText();
    }

    IEnumerator ActivateAndMoveRiverStartAfterDelay()
    {
        if (riverStartObject != null && riverStartPoint != null)
        {
            riverStartObject.transform.position = riverStartPoint.position;
            riverStartObject.SetActive(true);
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator ShowTurnDisplayBriefly()
    {
        UpdateTurnDisplay();
        turnDisplayText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);

        while (RiverFlow.instance == null || !RiverFlow.instance.IsCollidingWithRiverEnd())
        {
            if (RiverFlow.instance != null)
            {
                yield return new WaitForSeconds(3);
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }

        turnDisplayText.gameObject.SetActive(false);
        Beaver[] allBeavers = FindObjectsOfType<Beaver>();
        foreach (Beaver beaver in allBeavers)
        {
            beaver.ResetBeaver();
        }
    }
    void UpdateTurnDisplay()
    {
        turnDisplayText.text = "";
        turnDisplayText.text = "Ėjimas: " + turnCount;
    }

    void CheckWinCondition()
    {
        if ((currentLevelConfig.requiredFood <= 0 || ResourceManager.instance.food >= currentLevelConfig.requiredFood) &&
            (currentLevelConfig.requiredLodges <= 0 || BeaverManager.instance.lodgesBuilt >= currentLevelConfig.requiredLodges) &&
            (currentLevelConfig.requiredDams <= 0 || BeaverManager.instance.damsBuilt >= currentLevelConfig.requiredDams) &&
            (currentLevelConfig.requiredWood <= 0 || ResourceManager.instance.wood >= currentLevelConfig.requiredWood))
        {
            UnlockNewLevel();
            SceneManager.LoadScene(currentLevelConfig.nextScene);
        }
    }
    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }

    void CheckLoseCondition()
    {
        if (turnCount > loseTurnLimit || ResourceManager.instance.food < 0)
        {
            Debug.Log("You lose!");
            SceneManager.LoadScene("LoseScene");
        }
    }
    void DeductFoodForBeavers()
    {
        ResourceManager.instance.food -= currentLevelConfig.BeaverInLevel;
    }

}
