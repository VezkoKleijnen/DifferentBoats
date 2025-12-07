using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDataButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI myTextMesh;
    string myLocation;
    string myText;

    private void OnEnable()
    {
        if (myTextMesh == null)
        {
            myTextMesh.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    public void SetLocation(string location)
    {
        myLocation = location;
        myText = Path.GetFileName(location);
        if (myTextMesh != null)
        {
            myTextMesh.text = myText;
        }
        //set button name to the filename
    }
    public void LoadData()
    {
        SaveBoatStats.Instance.LoadStats(myLocation);
        SceneManager.LoadScene("DataDisplay");
    }
}
