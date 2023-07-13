using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PoolBall : NetworkBehaviour
{
    [SerializeField]
    private float _angularDrag = 0.6f;

    [SerializeField]
    private float _drag = 0.6f;

    private Rigidbody _rigidbody;
    private void Awake()
    {
        Debug.Log("white ball awake");
        _rigidbody = GetComponent<Rigidbody>();

    }

    private IEnumerator Start()
    {
        while (PlayerManager.Instance == null)
        {
            yield return null;
        }
        PlayerManager.Instance.OnPlayerTurnChanged.AddListener(OnPlayerTurnChanged);
    }

    private void OnPlayerTurnChanged(NetworkPlayer newPlayer)
    {

    }

    public override void Spawned()
    {
        Debug.Log("white ball spawned");

        Camera.main.GetComponent<LookAtConstraint>().AddSource(new ConstraintSource()
        { sourceTransform = transform, weight = 1f });

        PlayerManager.Instance.Rpc_LoadDone();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_PushBall(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction);

        StartCoroutine(CheckStopMoving());
    }

    public void OnMouseUpAsButton()
    {
        if (!PlayerManager.Instance.IsActivePlayerLocalPlayer())
            return;

        Vector3 direction = Random.onUnitSphere;
        direction.y = 0;
        direction.Normalize();

        Rpc_PushBall(direction * 2);
    }

    private IEnumerator CheckStopMoving()
    {
        while (!_rigidbody.IsSleeping())
        {
            yield return null;
        }

        PlayerManager.Instance.EndPlayerTurn();
    }
}
