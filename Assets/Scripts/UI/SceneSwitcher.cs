using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        if (SpeedController.IsValid())
        {
            SpeedController.Instance.Setup(10.0f, 2, 5, 100);
        }
        
        SceneManager.LoadScene(scene);
    }
}
