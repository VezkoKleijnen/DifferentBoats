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

    private List<List<int>> _triangles;

    private void Start()
    { 
        //sorry cant make proper mesh generator in time, it was hard for a random collection of 3d points to form a coherent mesh along the x and z axis, while y was displaying the performance, for now it shall just be points in a graph enviroment
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aliceBlue;
        foreach (Vector3 point in points)
        {
            Gizmos.DrawSphere(point, .4f);
        }
    }

}