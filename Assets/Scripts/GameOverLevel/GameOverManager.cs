using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{   
    public void Home()
    {
        SceneManager.LoadScene("Menu");
        Cursor.lockState = CursorLockMode.None;// fare imlecini kilidini
    }

    public void Restart_button()
    {
        Cursor.lockState = CursorLockMode.Locked;// fare imlecini kilitler
        Time.timeScale = 1.0f;

        if (MenuManager.bolum1==true)
        {         
            SceneManager.LoadScene("Level1");           
        }
        else
        {
            SceneManager.LoadScene("Level2");
        }       
    }

    public void Exit_button()
    {
        Application.Quit();
    }
}