using UnityEngine;
using UnityEngine.UI;

public class OnClickEvents : MonoBehaviour
{
    public Text soundsText;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.mute)
            soundsText.text = "/";
        else
            soundsText.text = "";
    }

    public void ToggleMute()
    {
        if(GameManager.mute)
        {
            GameManager.mute = false;
            soundsText.text = "";
        }
        else
        {
            GameManager.mute = true;
            soundsText.text = "/";
        }
    }

    public void ChangeEnvironmet(int selected)
    {
        PlayerPrefs.SetInt("CurrentEnvironment", selected);
        FindObjectOfType<EnvironmentSelector>().ChangeEnvironment(selected);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
