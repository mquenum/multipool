using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class SessionListManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private GameObject _sessionItemListPrefab;
    [SerializeField] private Button _sessionBrowserPanelButton;
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
    [SerializeField] private MenuManager _menuManager;
    [SerializeField] private SessionManager _sessionManager;

    private void Awake()
    {
        ClearList();
    }

    public void ClearList()
    {
        //Delete all children of the vertical layout group
        foreach (Transform child in _verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        //Hide the status message
        _statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        //Add a new item to the list
        SessionInfoListUIItem addedSessionInfoListUIItem = Instantiate(_sessionItemListPrefab, _verticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();

        addedSessionInfoListUIItem.SetInformation(sessionInfo);
        //Hook up events
        addedSessionInfoListUIItem.OnJoinSession += AddedSessionInfoListUIItem_OnJoinSession;
    }

    private void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo sessionInfo)
    {
        _sessionManager.StartGame(sessionInfo.Name);
        _menuManager.OnJoiningServer();
    }

    public void OnNoSessionsFound()
    {
        ClearList();

        _statusText.text = "No game session found";
        _statusText.gameObject.SetActive(true);
    }

    public void OnLookingForGameSessions(Button button = null)
    {
        ClearList();

        _statusText.text = "Looking for game sessions";
        _statusText.gameObject.SetActive(true);
    }

    public void ActivateButton()
    {
        _sessionBrowserPanelButton.interactable = true;
    }
}
