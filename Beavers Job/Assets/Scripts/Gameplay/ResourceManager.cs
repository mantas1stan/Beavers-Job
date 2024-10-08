using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int wood = 0;
    public int food = 0;
    public int stone = 0;

    void Start()
    {
        UIManager.instance.UpdateResourceUI(wood, food, stone);
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddResource(string type, int amount)
    {
        switch (type)
        {
            case "wood":
                wood += amount;
                break;
            case "food":
                food += amount;
                break;
            case "stone":
                stone += amount;
                break;
            default:
                Debug.LogError("Unknown resource type: " + type);
                break;
        }

        UIManager.instance.UpdateResourceUI(wood, food, stone);
        GameManager.instance.UpdateWinConditionText();
    }
}
