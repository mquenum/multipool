using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject playerDetailsPanel;
    public GameObject sessionBrowserPanel;
    public GameObject createSessionPanel;
    public GameObject statusPanel;

    [Header("Player settings")]
    public TMP_InputField playerNameInputField;

    [Header("New game session")]
    public TMP_InputField sessionNameInputField;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            playerNameInputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }


    public void OnFindGameClicked()
    {
        PlayerPrefs.SetString("PlayerNickname", playerNameInputField.text);
        PlayerPrefs.Save();

        GameManager.instance.playerNickName = playerNameInputField.text;

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobby();

        HideAllPanels();

        sessionBrowserPanel.gameObject.SetActive(true);
        FindObjectOfType<SessionListUIHandler>(true).OnLookingForGameSessions();
    }

    public void OnCreateNewGameClicked()
    {
        HideAllPanels();

        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionNameInputField.text, "01-PoolGame");

        HideAllPanels();
        statusPanel.GetComponent<TMP_Text>().text = "Loading game...";
        statusPanel.gameObject.SetActive(true);
    }

    public void OnJoiningServer()
    {
        HideAllPanels();

        statusPanel.gameObject.SetActive(true);
    }

    public void HideAllPanels()
    {
        playerDetailsPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
        statusPanel.SetActive(false);
        createSessionPanel.SetActive(false);
    }
}
