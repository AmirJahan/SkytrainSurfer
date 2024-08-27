using UnityEngine;

public class Quit : MonoBehaviour
{
    private void Update()
    {
        //this is literally only for testing on the computers
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}
