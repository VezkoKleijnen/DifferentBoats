using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ChartDisplay : MonoBehaviour
{
    List<Vector3> points = new List<Vector3>();
    List<int> triangless = new List<int>();
    private int amountToTest = 20;
    [SerializeField] private GameObject dataBall;

    private List<List<int>> _triangles;

    List<List<BoatData>> allBoatStatsData;
    List<GameObject> boatDataBalls = new List<GameObject>();
    [SerializeField] int checkValue = 1;

    int totalBoats;

    int currentGeneration = 0;

    private Vector3 pointsDirection;

    private void Start()
    {
        pointsDirection = (new Vector3(1,1,1)).normalized;
        //sorry cant make proper mesh generator in time, it was hard for a random collection of 3d points to form a coherent mesh along the x and z axis, while y was displaying the performance, for now it shall just be points in a graph enviroment
        allBoatStatsData = SaveBoatStats.Instance.GetAllData();
        int total = 0;
        totalBoats = SaveBoatStats.Instance.GetAmountOfBoats();

        for (int i = 0; i < totalBoats; i++)
        {
            boatDataBalls.Add(Instantiate(dataBall, this.transform));
        }
        UpdateList(currentGeneration, ATTACK, DEFENSE, SPEED);
    }

    public static int ATTACK = 0;
    public static int DEFENSE = 1;
    public static int SPEED = 2;
    public static int POINTS = 3;
    
    private void UpdateList(int generation, int xAxis, int zAxis, int yAxis)
    {
        float highestPoints = 0;
        int howManyBalls = 0;
        for (int i = 0; i < totalBoats; i++)
        {
            GoToPos goToPos = boatDataBalls[i].GetComponent<GoToPos>();
            Vector3 destination = new Vector3(GetValue(generation, i, xAxis), GetValue(generation, i, yAxis), GetValue(generation, i, zAxis));
            //destination += GetValue(generation, i, POINTS) * pointsDirection / 2;
            if (goToPos == null)
            {
                boatDataBalls[i].transform.position = destination;
            }
            else
            {
                goToPos.SetDestination(destination);
                goToPos.SetStats(GetValue(generation, i, ATTACK), GetValue(generation, i, DEFENSE), GetValue(generation, i, SPEED), GetValue(generation, i, POINTS));
            }



            if (GetValue(generation, i, POINTS) > highestPoints)
            {
                highestPoints = GetValue(generation, i, POINTS);
            }
            if (i == checkValue)
            {
                UnityEngine.Debug.Log("points is: " + GetValue(generation, i, POINTS));
            }
            howManyBalls++;
        }
        for (int i = 0; i < totalBoats; i++)
        {

            foreach(Transform child in boatDataBalls[i].GetComponentsInChildren<Transform>())
            {
                child.GetComponent<MeshRenderer>().material.color =
                new Color(
                GetValue(generation, i, POINTS) / highestPoints,
                GetValue(generation, i, POINTS) / highestPoints,
                GetValue(generation, i, POINTS) / highestPoints
                );
            }
        }
        UnityEngine.Debug.Log(howManyBalls);
    }
    private float GetValue(int generation, int boat, int statType)
    {
        //int offset = totalBoats * generation;
        switch (statType)
        {
            case 0: //ATTACK
                return allBoatStatsData[generation][boat].attack;
            case 1: //DEFENSE
                return allBoatStatsData[generation][boat].defense;
            case 2:
                return allBoatStatsData[generation][boat].speed;
            case 3:
                return allBoatStatsData[generation][boat].points;   
        }
        UnityEngine.Debug.LogWarning("out of range stat type? " + statType);
        return 0;
    }
    public void SetGeneration(float input)
    {
        input = Mathf.Clamp01(input);
        int newValue = Mathf.FloorToInt(input * allBoatStatsData.Count);

        // ensure 1.0 maps to the last index
        newValue = Mathf.Min(newValue, allBoatStatsData.Count - 1);

        if (currentGeneration != newValue)
        {
            currentGeneration = newValue;
            UpdateList(currentGeneration, ATTACK, DEFENSE, SPEED);
        }
    }
    /*    private void OnDrawGizmos()
        {
            Gizmos.color = Color.aliceBlue;
            foreach (Vector3 point in points)
            {
                Gizmos.DrawSphere(point, .4f);
            }
        }*/

}