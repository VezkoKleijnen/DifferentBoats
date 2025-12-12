using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ChartDisplay : MonoBehaviour
{
    List<Vector3> points = new List<Vector3>();
    List<int> triangless = new List<int>();
    private int amountToTest = 20;
    [SerializeField] private GameObject dataBall;
    [SerializeField] private GameObject graphPrefab;
    [SerializeField] private Material attackMaterial;
    [SerializeField] private Material defenseMaterial;
    [SerializeField] private Material speedMaterial;
    [SerializeField] private Material pointsMaterial;
    [SerializeField] private float graphScale = .005f;

    List<List<BoatData>> allBoatStatsData;
    List<GameObject> boatDataBalls = new List<GameObject>();

    int totalBoats;
    int totalGenerations;

    int currentGeneration = 0;

    private Vector3 pointsDirection;

    public static bool graphMode = false;

    List<Vector2> attackGraph = new List<Vector2>();
    List<Vector2> defenseGraph = new List<Vector2>();
    List<Vector2> speedGraph = new List<Vector2>();
    List<Vector2> pointsGraph = new List<Vector2>();
    int totalAmount = 0;
    private void Start()
    {
        pointsDirection = (new Vector3(1, 1, 1)).normalized;
        if (SaveBoatStats.Instance == null)
        {
            SceneManager.LoadScene("LoadDataScene");
            return;
        }
        allBoatStatsData = SaveBoatStats.Instance.GetAllData();
        totalBoats = SaveBoatStats.Instance.GetAmountOfBoats();
        totalGenerations = SaveBoatStats.Instance.GetAmountOfGenerations();
        StartTable();
        UpdateList(currentGeneration, ATTACK, DEFENSE, SPEED);
    }

    public static int ATTACK = 0;
    public static int DEFENSE = 1;
    public static int SPEED = 2;
    public static int POINTS = 3;
    
    private void UpdateGraph()
    {

    }
    private void UpdateList(int generation, int xAxis, int zAxis, int yAxis)
    {
        float highestPoints = 0;
        int howManyBalls = 0;
        for (int i = 0; i < totalBoats; i++)
        {
            TableBehavior goToPos = boatDataBalls[i].GetComponent<TableBehavior>();
            Vector3 destination = new Vector3(GetValue(generation, i, xAxis), GetValue(generation, i, yAxis), GetValue(generation, i, zAxis));
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
    }
    private float GetValue(int generation, int boat, int statType)
    {
        //int offset = totalBoats * generation;
        if (generation >= totalGenerations)
        {
            UnityEngine.Debug.LogError("OUT OF GENERATION RANGE");
            //it keeps going out of range here when generating the graph
            UnityEngine.Debug.Log(generation);
            return 0;
        }
        if (boat >= totalBoats)
        {
            UnityEngine.Debug.LogError("OUT OF GENERATION RANGE");
            return 0;
        }
        switch (statType)
        {
            case 0: //ATTACK
                return allBoatStatsData
                    [generation]
                    [boat]
                    .attack;
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
            if (!graphMode)
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

    public void SwitchMode()
    {
        graphMode = !graphMode;
        if (graphMode)
        {
            StartGraph();
            UpdateGraph();
        }
        else
        {
            StartTable();
            UpdateList(currentGeneration, ATTACK, DEFENSE, SPEED);
        }
    }
    private void StartTable()
    {
        RestartDataBalls();
        for (int i = 0; i < totalBoats; i++)
        {
            boatDataBalls.Add(Instantiate(dataBall, this.transform));
        }
    }
    private void StartGraph()
    {
        RestartDataBalls();
        int rememberGeneration = currentGeneration;
        for (int i = 0; i < totalGenerations; i++)
        {
            currentGeneration = i;
            float avarageAttack = 0;
            float avarageDefense = 0;
            float avarageSpeed = 0;
            float avaragePoints = 0;

            float totalAttack = 0;
            float totalDefense = 0;
            float totalSpeed = 0;
            float totalPoints = 0;

            int totalSucces = 0;

            for (int j = 0; j < totalBoats; j++)
            {
                if (GetValue(i, j, POINTS) > 0)
                {
                    totalAttack += GetValue(i, j, ATTACK);
                    totalDefense += GetValue(i, j, DEFENSE);
                    totalSpeed += GetValue(i, j, SPEED);
                    totalPoints += GetValue(i, j, POINTS);
                    totalSucces++;
                }
            }
            avarageAttack = totalAttack / totalSucces;
            avarageDefense = totalDefense / totalSucces;
            avarageSpeed = totalSpeed / totalSucces;
            avaragePoints = totalPoints / totalSucces;

            attackGraph.Add(new Vector2(currentGeneration * graphScale, avarageAttack));
            defenseGraph.Add(new Vector2(currentGeneration * graphScale, avarageDefense));
            speedGraph.Add(new Vector2(currentGeneration * graphScale, avarageSpeed));
            pointsGraph.Add(new Vector2(currentGeneration* graphScale, avaragePoints));
            //x dependent on current generation (check total generation en deel dat door een total afstand waarop je de graph wil, of doe het in verhouding tot de slider
            //geef iedere stat een lijn in de designated kleur
        }
        UnityEngine.Debug.Log("got thru the shit above");
        for (int i = 0; i < totalGenerations-1; i++) //-1 is so that is stops before the last one, because there is no next one after the last one
        {
            GenerateGraphLine(new Vector3(0, attackGraph[i].y, attackGraph[i].x), new Vector3(0, attackGraph[i+1].y, attackGraph[i + 1].x), ATTACK);
            GenerateGraphLine(new Vector3(0, defenseGraph[i].y, defenseGraph[i].x), new Vector3(0, defenseGraph[i + 1].y, defenseGraph[i + 1].x), DEFENSE);
            GenerateGraphLine(new Vector3(0, speedGraph[i].y, speedGraph[i].x), new Vector3(0, speedGraph[i + 1].y, speedGraph[i + 1].x), SPEED);
            GenerateGraphLine(new Vector3(0, pointsGraph[i].y, pointsGraph[i].x), new Vector3(0, pointsGraph[i + 1].y, pointsGraph[i + 1].x), POINTS);
        }

        currentGeneration = rememberGeneration;


    }
    private void GenerateGraphLine(Vector3 start, Vector3 end, int typeOfGraph)
    {
        totalAmount++;
        GameObject newLine = Instantiate(graphPrefab, start, Quaternion.identity);
        Vector3 direction = (end - start).normalized;
        newLine.transform.forward = direction;
        newLine.transform.localScale = new Vector3(1, 1, (start - end).magnitude);
        MeshRenderer renderer = newLine.GetComponentInChildren<MeshRenderer>();
        switch (typeOfGraph)
        {
            case 0:
                renderer.material = attackMaterial;
                break;
            case 1:
                renderer.material = defenseMaterial;
                break;
            case 2:
                renderer.material = speedMaterial;
                break;
            case 3:
                renderer.material = pointsMaterial;
                break;
        }
        boatDataBalls.Add(newLine);
    }
    private void RestartDataBalls()
    {
        for (int i = boatDataBalls.Count - 1; i >= 0; i--)
        {
            Destroy(boatDataBalls[i]);
            boatDataBalls.RemoveAt(i);
        }
    }

}