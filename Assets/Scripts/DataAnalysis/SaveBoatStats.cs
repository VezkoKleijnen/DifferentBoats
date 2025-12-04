using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class BoatStatsData
{
    public uint amountOfBoats;
    public int amountOfGenerations;
    public List<float> attacks;
    public List<float> defenses;
    public List<float> speeds;
    public List<float> points;
}
public class SaveBoatStats
{
    public static SaveBoatStats Instance;
    public SaveBoatStats()
    {
        Instance = this;
    }
    uint amountOfBoats = 0;
    int amountOfGenerations = 0;
    List<float> attacks = new List<float>();
    List<float> defenses = new List<float>();
    List<float> speeds = new List<float>();
    List<float> points = new List<float>();

    public void SetBoatsAmount(uint amountOfBoats)
    {
        this.amountOfBoats = amountOfBoats;
    }

    public void AddStats(BoatLogic boat)
    {
        //if (amountOfGenerations == 0) amountOfBoats++;
        attacks.Add(boat.attack);
        defenses.Add(boat.defense);
        speeds.Add(boat.speed);
        points.Add(boat.GetPoints());
    }
    public void AddStats(List<BoatLogic> boats)
    {
        foreach(BoatLogic boat in boats)
        {
            //if (amountOfGenerations == 0) amountOfBoats++;
            attacks.Add(boat.attack);
            defenses.Add(boat.defense);
            speeds.Add(boat.speed);
            points.Add(boat.GetPoints());
        }
        Debug.Log(amountOfBoats);
    }
    public void StartNewGeneration()
    {
        amountOfGenerations++;
    }
    public void SaveStats()
    {
        if (amountOfGenerations <= 0)
            return;
        //first write the 2 ints so that the next time the file is loaded the project knows how many boat there are in 1 generation and how many generations there are in the file
        //than write all the floats in a clear order so the next time the program reads it it is easy to get the information out again
        BoatStatsData data = new BoatStatsData
        {
            amountOfBoats = amountOfBoats,
            amountOfGenerations = amountOfGenerations,
            attacks = attacks,
            defenses = defenses,
            speeds = speeds,
            points = points
        };

        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "boatStats.json");
        File.WriteAllText(path, json);

        Debug.Log("Stats saved to: " + path);
    }
}
