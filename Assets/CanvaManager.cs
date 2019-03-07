using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvaManager : MonoBehaviour
{
    public GameObject LoosePanel;
    public GameObject PausePanel;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI FinalScoreText;

    public void SetScore(int score)
    {
        ScoreText.text = "x "+score.ToString();
    }

    public void Pause()
    {
        GameManager.instance.Pause();
        PausePanel.SetActive(true);
    }

    public void Restart()
    {
        PausePanel.SetActive(false);
        GameManager.instance.Restart();
    }

    public void ReturnTo(string levelScene)
    {
        PausePanel.SetActive(false);
        GameManager.instance.ReturnTo(levelScene);
    }

    public void ShowLooseScreen()
    {
        PausePanel.SetActive(false);
        LoosePanel.SetActive(true);
        FinalScoreText.text = ScoreText.text;
    }
}
