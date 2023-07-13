using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
    private NetworkPlayer _networkPlayer;
    private Vector2 moveInputVector = Vector2.zero;
    private Vector2 viewInputVector = Vector2.zero;
    private bool _mouseButton0;
    private bool _mouseButton1;
    private float _accumulatedDelta = 0;

    // Start is called before the first frame update
    void Start()
    {
        _networkPlayer = GetComponent<NetworkPlayer>();


    }

    // Update is called once per frame
    void Update()
    {
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1; //Invert the mouse look

        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 || Input.GetMouseButton(1);

        _accumulatedDelta += Input.GetAxis("Mouse Y");

        if (Input.GetMouseButtonUp(0))
        {
            // Dragging stopped
            Debug.Log("Dragging stopped!");
            PlayerManager.Instance.EndPlayerTurn();
            _accumulatedDelta = 0;
        }
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //Move data
        networkInputData.movementInput = moveInputVector;

        //View data
        networkInputData.rotationInput = viewInputVector.x;

       /* if (_mouseButton0)
            networkInputData.buttons |= NetworkInputData.MOUSEBUTTON1;
        _mouseButton0 = false;

        if (_mouseButton1)
            networkInputData.buttons |= NetworkInputData.MOUSEBUTTON2;
        _mouseButton1 = false;*/

        if (networkInputData.isDragging = Input.GetMouseButton(0))
        {
            Debug.Log($"{_networkPlayer.NetworkPlayerRef} is local player ? {_networkPlayer.IsLocalPlayer()}");
            Debug.Log("networkInputData.isDragging = Input.GetMouseButton(0)");
            networkInputData.dragDelta = _accumulatedDelta;
        }

        return networkInputData;
    }
}
