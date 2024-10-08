using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/New Level Configuration")]
public class LevelConfig : ScriptableObject
{
    public int requiredFood;
    public int requiredLodges;
    public int requiredDams;
    public int requiredWood;
    public int maxTurns;
    public int BeaverInLevel;
    public string nextScene;
}