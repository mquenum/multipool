using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Fusion;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SessionManager _sessionManager;
    [SerializeField] private SessionListManager _sessionListManager;

    [Header("Panels")]
    [SerializeField] private GameObject _playerDetailsPanel;
    [SerializeField] private GameObject _sessionBrowserPanel;
    [SerializeField] private GameObject _createSessionPanel;
    [SerializeField] private GameObject _statusPanel;

    [Header("Player settings")]
    [SerializeField] private TMP_InputField _playerNameInputField;

    [Header("New game session")]
    [SerializeField] private TMP_InputField _sessionNameInputField;

    public void OnFindGameClicked()
    {
        Debug.Log("OnFindGameClicked");

        _sessionManager.OnJoinLobby();
        HideAllPanels();
        _sessionBrowserPanel.gameObject.SetActive(true);
        _sessionListManager.OnLookingForGameSessions();
    }

    public void OnCreateNewGameClicked()
    {
        HideAllPanels();
        _createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        _sessionManager.StartGame(_sessionNameInputField.text);
        HideAllPanels();
        _statusPanel.GetComponent<TMP_Text>().text = "Loading game...";
        _statusPanel.gameObject.SetActive(true);
    }

    public void OnJoiningServer()
    {
        HideAllPanels();
        _statusPanel.gameObject.SetActive(true);
    }

    public void HideAllPanels()
    {
        _playerDetailsPanel.SetActive(false);
        _sessionBrowserPanel.SetActive(false);
        _statusPanel.SetActive(false);
        _createSessionPanel.SetActive(false);
    }
}
