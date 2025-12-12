using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneLoader : MonoBehaviour
{
    public void LoadMyScene()
    {
        SceneManager.LoadScene("LoadDataScene");
    }
}
