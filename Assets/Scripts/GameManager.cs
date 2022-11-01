using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance { set; get; }
    public static bool mute = false;

    public bool IsDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMotor motor;
    private int lastScore;
    private AudioManager audioManager;

    // UI and UI fields
    public Animator menuAnim;
    public Animator gameCanvas;
    public Text scoreText, coinText, modifierText, hiScoreText;
    private float score, coinScore, modifierScore;

    // Death menu
    public Animator deathMenuAnim;
    public Text deadScoreText, deadCoinText;

    private void Awake()
    {
        Instance = this;
        modifierScore = 1.0f;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        modifierText.text = "x" + modifierScore.ToString("0.0");
        scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString();
        audioManager = FindObjectOfType<AudioManager>();

        hiScoreText.text = PlayerPrefs.GetInt("Hiscore").ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            // moved to OnPlay function
            //OnPlay();
        }

        if (isGameStarted && !IsDead)
        {
            // Bump the score up
            lastScore = (int)score;
            score += (Time.deltaTime * modifierScore);
            if (lastScore == (int)score)
            {
                scoreText.text = score.ToString("0");
            }
            
        }
    }

    public void GetCoin()
    {
        coinScore++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT;
        scoreText.text = score.ToString("0");
    }
    
    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnPlay()
    {
        isGameStarted = true;
        motor.StartRunning();
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraMotor>().IsMoving = true;
        gameCanvas.SetTrigger("Show");
        menuAnim.SetTrigger("Hide");
        audioManager.Play("background");
    }

    public void OnDeath()
    {
        IsDead = true;
        audioManager.Stop("background");
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        deadScoreText.text = score.ToString("0");
        deadCoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide");

        // Check if this is a highscore
        if (score > PlayerPrefs.GetInt("Hiscore"))
        {
            float s = score;
            if (s % 1 == 0)
                s += 1;
            PlayerPrefs.SetInt("Hiscore", (int)s);
        }
    }
}
