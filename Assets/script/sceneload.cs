using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneload : MonoBehaviour
{
    public void scenecahnge(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
