using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UI_Main ui;
    public static GameManager instance;
    public Player player;

    public Color platformColor;



    public int coins;
    public float distance;
    public float score;


    public bool isMoving;
    private Coroutine checkMovementCoroutine;


    private void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        //LoadColor();
    }

    private void Start()
    {
        PlayerPrefs.GetInt("Coins");
    }

    private void Update()
    {
        if (player.transform.position.x > distance)
            distance = player.transform.position.x;

        NoSpeedEndGame();
    }

    private void NoSpeedEndGame()
    {
        if (player.rb.velocity.x > 0.01f)
        {
            isMoving = true;
            if (checkMovementCoroutine != null)
            {
                StopCoroutine(checkMovementCoroutine);
                checkMovementCoroutine = null;
            }
        }
        else if (isMoving)
        {
            isMoving = false;
            checkMovementCoroutine = StartCoroutine(CheckMovement());
        }
    }

    public void UnlockPlayer() => player.runStart = true;
    public void RestartLevel()
    {
        SaveInfo();
        SceneManager.LoadScene(0);
    }

    public void SaveInfo()
    {
        int savedCoins = PlayerPrefs.GetInt("Coins");

        PlayerPrefs.SetInt("Coins", savedCoins + coins);

        score = (distance * coins) / 10;

        PlayerPrefs.SetFloat("LastScore", score);

        if (PlayerPrefs.GetFloat("HighScore") < score)
            PlayerPrefs.SetFloat("HighScore", score);
    }
    public void SaveColor(float r, float g, float b)
    {
        PlayerPrefs.SetFloat("ColorR", r);
        PlayerPrefs.SetFloat("ColorG", g);
        PlayerPrefs.SetFloat("ColorB", b);
    }

    private void LoadColor()
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        Color newColor = new Color(PlayerPrefs.GetFloat("ColorR"),
                                  PlayerPrefs.GetFloat("ColorG"),
                                  PlayerPrefs.GetFloat("ColorB"),
                                  PlayerPrefs.GetFloat("ColorA", 1));

        sr.color = newColor;

    }


    public void GameEnded()
    {
        SaveInfo();
        ui.OpenEndGameUI();
    }

    IEnumerator CheckMovement()
    {
        yield return new WaitForSeconds(3);
        if (!isMoving && player.runStart == true)
        {
            GameEnded();
        }
    }
}
