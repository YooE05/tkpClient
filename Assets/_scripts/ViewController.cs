using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ViewController : MonoBehaviour
{
    GameController gameController;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI laserText;
    public GameObject exitButton;

    public GameObject gameEndPanel;
    public GameObject gameView;

    public GameObject _loadingScreen;
    public GameObject _loadingEndScreen;


    void Start()
    {
        _loadingScreen.SetActive(true);
        gameEndPanel.SetActive(false);        
        ShowGameView();

        GameEvents.current.OnIncreasePoints += IncreasePoints;
        GameEvents.current.OnDamagedPlayer += ReduceHealth;
        gameController = FindObjectOfType<GameController>();
    }

    


    //убрать пред релизом 
    public void RegenerateScreen()
    {
        gameController.GenerateLevelStruct();
    }

    void ReduceHealth()
    {
        healthText.text = (Convert.ToInt32(healthText.text) - 1).ToString();
    }

    void IncreasePoints()
    {
        pointsText.text = (Convert.ToInt32(pointsText.text) + 1).ToString();
    }

    public void ShowExitButton()
    {
        exitButton.SetActive(true);
    }
    public void ChangeLaserDisableText(bool state)
    {
        laserText.gameObject.SetActive(state);
    }

    public void ShowGameView()
    {
        gameView.SetActive(true);
    }
    public void HideGameView()
    {
        gameView.SetActive(false);
    }
    public void RestartGameView(int startHP)
    {
        ShowGameView();
        gameEndPanel.SetActive(false);

        pointsText.text = "0";
        healthText.text = startHP.ToString();
        exitButton.SetActive(false);
    }

    internal void ShowLoadingView()
    {
        _loadingScreen.SetActive(true);
    }
    public void HideLoadingView()
    {
        _loadingScreen.SetActive(false);
    }

    internal void ShowEndLoadingView()
    {
       _loadingEndScreen.SetActive(true);
    }
    public void HideEndLoadingView()
    {
        _loadingEndScreen.SetActive(false);
    }

    public void ShowGameEndPanel(int startHp, int maxPoints)
    {

        gameEndPanel.transform.Find("MessagePanel/Health").gameObject.GetComponent<TextMeshProUGUI>().text = healthText.text;
        gameEndPanel.transform.Find("MessagePanel/Points").gameObject.GetComponent<TextMeshProUGUI>().text = pointsText.text;
        HideGameView();

        gameEndPanel.SetActive(true);
        gameEndPanel.transform.Find("MessagePanel/Health/StartCountOfHealth").gameObject.GetComponent<TextMeshProUGUI>().text = "/ " + startHp;
        gameEndPanel.transform.Find("MessagePanel/Points/MaxCountOfPoints").gameObject.GetComponent<TextMeshProUGUI>().text = "/ " + maxPoints;
    }



    private void OnDestroy()
    {
        GameEvents.current.OnIncreasePoints -= IncreasePoints;
        GameEvents.current.OnDamagedPlayer -= ReduceHealth;
    }

}
