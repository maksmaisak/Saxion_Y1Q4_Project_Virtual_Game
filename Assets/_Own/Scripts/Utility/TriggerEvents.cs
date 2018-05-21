using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// Exposes OnTriggerEnter as a UnityEvent.
public class TriggerEvents : MonoBehaviour
{
    [System.Serializable]
    public class TriggerEvent : UnityEvent<Collider> {}

    [SerializeField] TriggerEvent _onTriggerEnter = new TriggerEvent();
    public TriggerEvent onTriggerEnter { get { return _onTriggerEnter; } }

    [SerializeField] UnityEvent _onPlayerTriggerEnter = new UnityEvent();
    public UnityEvent onPlayerTriggerEnter { get { return _onPlayerTriggerEnter; } }

    [SerializeField] UnityEvent _onPlayerTriggerStay = new UnityEvent();
    public UnityEvent onPlayerTriggerStay { get { return _onPlayerTriggerStay; } }

    [SerializeField] UnityEvent _onPlayerTriggerExit = new UnityEvent();
    public UnityEvent onPlayerTriggerExit { get { return _onPlayerTriggerExit; } }

    void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);

        if (IsPlayer(other))
        {
            onPlayerTriggerEnter.Invoke();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (IsPlayer(other))
        {
            onPlayerTriggerStay.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            onPlayerTriggerExit.Invoke();
        }
    }

    private bool IsPlayer(Collider other)
    {
        GameObject playerGameObject = Player.Instance.gameObject;
        if (other.gameObject == playerGameObject) return true;
        if (other.attachedRigidbody != null && other.attachedRigidbody.gameObject == playerGameObject) return true;
        return false;
    }
}
