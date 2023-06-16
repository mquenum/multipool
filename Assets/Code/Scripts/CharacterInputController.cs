using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    private bool _mouseButton0;
    private bool _mouseButton1;
    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //Move data
        networkInputData.movementInput = moveInputVector;

        //View data
        networkInputData.rotationInput = viewInputVector.x;

        if (_mouseButton0)
            networkInputData.buttons |= NetworkInputData.MOUSEBUTTON1;
        _mouseButton0 = false;

        if (_mouseButton1)
            networkInputData.buttons |= NetworkInputData.MOUSEBUTTON2;
        _mouseButton1 = false;

        return networkInputData;
    }
}
