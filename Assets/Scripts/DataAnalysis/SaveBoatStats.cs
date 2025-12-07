using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class BoatStatsData
{
    public int amountOfBoats;
    public int amountOfGenerations;
    public List<float> attacks;
    public List<float> defenses;
    public List<float> speeds;
    public List<float> points;
}
public class BoatData
{
    public float attack;
    public float defense;
    public float speed;
    public float points;
    public BoatData(float attack, float defense, float speed, float points)
    {
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.points = points;
    }
}
public class SaveBoatStats
{
    public static SaveBoatStats Instance;
    public SaveBoatStats()
    {
        Instance = this;
    }
    int amountOfBoats = 0;
    int amountOfGenerations = 0;
    List<float> attacks = new List<float>();
    List<float> defenses = new List<float>();
    List<float> speeds = new List<float>();
    List<float> points = new List<float>();

    private int testCount = 0;

    private string globalDataPath = Application.dataPath + "/simData";

    public void SetBoatsAmount(int amountOfBoats)
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
    private void RemoveUnfinishedGeneration()
    {
        int boatCount = amountOfGenerations * amountOfBoats;
        int totalBoats = attacks.Count;
        for (int i = totalBoats; i > boatCount; i--)
        {
            attacks.RemoveAt(boatCount);
            defenses.RemoveAt(boatCount);
            speeds.RemoveAt(boatCount);
            points.RemoveAt(boatCount);
        }
        totalBoats = attacks.Count;
    }
    public void SaveStats()
    {
        RemoveUnfinishedGeneration();
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

            string fileName;
            string filenamePath = Path.Combine(globalDataPath, "filename.json");
            if (File.Exists(filenamePath))
            {
                string filenameText = File.ReadAllText(filenamePath);
                testCount = int.Parse(filenameText);

                testCount++;
            }
            fileName = "Simulation_" + testCount.ToString() + ".simData";
            File.WriteAllText(filenamePath, testCount.ToString()); //can be written cleaner sorry

        string path = Path.Combine(globalDataPath, fileName);
        File.WriteAllText(path, json);

        Debug.Log("Stats saved to: " + path);
        GetSimDataFiles(globalDataPath);
    }

    public void LoadStats(string pathToLoad)
    {
        if (File.Exists(pathToLoad))
        {
            string json = File.ReadAllText(pathToLoad);
            BoatStatsData data = JsonUtility.FromJson<BoatStatsData>(json);

            amountOfBoats = data.amountOfBoats;
            amountOfGenerations = data.amountOfGenerations;
            attacks = data.attacks;
            defenses = data.defenses;
            speeds = data.speeds;
            points = data.points;

            Debug.Log("Stats loaded from: " + pathToLoad);
        }
        else
        {
            Debug.LogWarning("No stats file found at: " + pathToLoad);
        }
    }
    public List<string> GetAllSimData()
    {
        return GetSimDataFiles(globalDataPath);
    }

    public List<string> GetSimDataFiles(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("folder Not Found: " + folderPath);
            return new List<string>();
        }

        string[] files = Directory.GetFiles(folderPath, "*.simData");
        return new List<string>(files);
    }
    public List<BoatData> GetData(int generation)
    {
        List<BoatData> returnData = new List<BoatData>();
        if (generation >= amountOfGenerations)
            return returnData;

        for (int i  = 0; i < amountOfBoats; i++)
        {
            int genI = i + (generation * amountOfBoats);
            if (genI >= attacks.Count || genI >= points.Count || genI >= speeds.Count || genI >= defenses.Count)
            {
                Debug.LogError("out of range at: " + genI);
                Debug.Log("attacks: " + attacks.Count);
                Debug.Log("points: " + points.Count);
                Debug.Log("defenses: " + defenses.Count);
                Debug.Log("speeds: " + speeds.Count);
                break;
            }
            else
            {
                returnData.Add(new BoatData(
                    attacks[genI],
                    defenses[genI],
                    speeds[genI],
                    points[genI]));
            }
        }
        return returnData;
    }
    public List<List<BoatData>> GetAllData()
    {
        List<List<BoatData>> returnData = new List<List<BoatData>>();
        for (int i = 0; i < amountOfGenerations; i++)
        {
            returnData.Add(GetData(i));
        }

        return returnData;
    }
    public int GetAmountOfBoats()
    {
        return amountOfBoats;
    }
}
