using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LoadBoatStats : MonoBehaviour
{
    [SerializeField] GameObject loadDataButton;
    SaveBoatStats saveData = new SaveBoatStats();
    List<string> simDataFiles;
    private void Start()
    {
        simDataFiles = saveData.GetAllSimData();
        foreach (string simDataFile in simDataFiles)
        {
            GameObject obj = Instantiate(
                loadDataButton, 
                this.transform);
            LoadDataButton dataButtonData = obj.GetComponent<LoadDataButton>();
            if (dataButtonData != null )
            {
                dataButtonData.SetLocation(simDataFile);
            }
            else
            {
                Destroy(obj);
            }
        }
    }
}
