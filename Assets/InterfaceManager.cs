using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class InterfaceManager : MonoBehaviour
{

    public GameObject inGameCanvas, GameOverCanvas;
    public Text textWin;
    public Image[] slowPowersP1 = new Image[3];
    public Image[] jumpPowersP1 = new Image[3];
    public Image[] musicPowersP1 = new Image[3];
    public Image[] jumpPowersP2 = new Image[3];
    public Image[] slowPowersP2 = new Image[3];
    public Image[] musicPowersP2 = new Image[3];

    public Text sceneText;

    int SceneCount = 0;

    public string[] sceneNames = new string[5];

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeMusicFoward()
    {
        SceneCount = (SceneCount + 1) % 5;
        sceneText.text = sceneNames[SceneCount];
    }
    public void ChangeMusicBackwards()
    {
        SceneCount = ((SceneCount - 1) + 5) % 5;
        sceneText.text = sceneNames[SceneCount];
    }

    public void PlayScene()
    {
        SceneManager.LoadScene(SceneCount + 1);
    }

    public void GameOver(int player)
    {
        if (player == 0)
            textWin.text = "Player 2 Win!!";
        else
            textWin.text = "Player 1 Win!!";

        inGameCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void addPower(int power, int player, int amount)
    {
        switch (power)
        {
            case 0:
                if(player == 0)
                    slowPowersP1[amount].enabled = true;
                else
                    slowPowersP2[amount].enabled = true;
                break;
            case 1:
                if (player == 0)
                    jumpPowersP1[amount].enabled = true;
                else
                    jumpPowersP2[amount].enabled = true;
                break;
            case 2:
                if (player == 0)
                    musicPowersP1[amount].enabled = true;
                else
                    musicPowersP2[amount].enabled = true;
                break;
        }
    }

    public void RemoveAll(int player)
    {
        for (int i = 0; i < 3; i++)
        {
            if (player == 0)
            {
                slowPowersP1[i].enabled = false;
                jumpPowersP1[i].enabled = false;
                musicPowersP1[i].enabled = false;
            }
            else
            {
                slowPowersP2[i].enabled = false;
                jumpPowersP2[i].enabled = false;
                musicPowersP2[i].enabled = false;
            }
        }
    }

}
