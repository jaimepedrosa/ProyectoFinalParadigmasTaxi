using UnityEngine;
using UnityEngine.SceneManagement;

public class PantallaFin : MonoBehaviour
{
    void Start()
    {
        Invoke("LoadMainMenu", 5);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
