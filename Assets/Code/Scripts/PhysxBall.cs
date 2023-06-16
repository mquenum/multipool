using UnityEngine;
using Fusion;

public class PhysxBall : NetworkBehaviour
{
    public Rigidbody rb;
    [Networked]
    NetworkInputData CurrInput { get; set; }
    [Networked]
    float PuttStrength { get; set; }
    [Networked]
    public TickTimer PuttTimer { get; set; }
    public bool CanPutt => PuttTimer.ExpiredOrNotRunning(Runner);
    Angle yaw = default;
    public float maxPuttStrength = 10;
    public float puttGainFactor = 0.1f;
    NetworkInputData prevInput = default;

    CharacterInputController characterInputController;

    public override void FixedUpdateNetwork()
    {
        if (characterInputController == null && NetworkPlayer.Local != null)
        {
            characterInputController = NetworkPlayer.Local.GetComponent<CharacterInputController>();
        }

        CurrInput = characterInputController.GetNetworkInput();

        if (CurrInput.isDragging)
        {
            PuttStrength = Mathf.Clamp(PuttStrength - (CurrInput.dragDelta * puttGainFactor), 0, maxPuttStrength);
        }

        if (CurrInput.isDragging == false && prevInput.isDragging)
        {
            Debug.Log("CurrInput.isDragging == false && prevInput.isDragging");
            if (CanPutt && PuttStrength > 0)
            {
                Debug.Log("CanPutt && PuttStrength > 0");
                Vector3 fwd = Quaternion.AngleAxis((float)CurrInput.yaw, Vector3.up) * Vector3.forward;

                rb.AddForce(fwd * PuttStrength, ForceMode.VelocityChange);
            }
        }

        prevInput = CurrInput;
    }
}